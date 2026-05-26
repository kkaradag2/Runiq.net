using System.Runtime.CompilerServices;
using System.Text;
using Runiq.Agents;
using Runiq.Agents.Runtime;
using Runiq.Agents.Tools;
using Runiq.Teams.Models.Execution;
using Runiq.Teams.Models.Teams;

namespace Runiq.Teams.Execution;

/// <summary>
/// Kayıtlı agent team tanımlarını sıralı multi-agent yürütme modeliyle çalıştıran runtime servisidir.
/// </summary>
public sealed class TeamExecutionRuntime
{
    private readonly IReadOnlyList<AgentTeam> teams;
    private readonly AgentExecutionRuntime agentRuntime;
    private readonly AgentToolInvoker toolInvoker;

    /// <summary>
    /// Yeni bir team execution runtime örneği oluşturur.
    /// </summary>
    /// <param name="teams">Runtime tarafından çalıştırılabilecek kayıtlı agent team koleksiyonudur.</param>
    /// <param name="agentRuntime">Takım üyelerini çalıştırmak için kullanılacak agent runtime örneğidir.</param>
    /// <param name="toolInvoker">Agent tool çağrılarını çalıştıran invoker örneğidir.</param>
    public TeamExecutionRuntime(
        IReadOnlyList<AgentTeam>? teams,
        AgentExecutionRuntime agentRuntime,
        AgentToolInvoker toolInvoker)
    {
        this.teams = teams ?? [];
        this.agentRuntime = agentRuntime;
        this.toolInvoker = toolInvoker;
    }

    /// <summary>
    /// Agent team cevabını team kimliğine göre event stream olarak üretir.
    /// </summary>
    /// <param name="teamId">Çalıştırılacak agent team kimliğidir.</param>
    /// <param name="input">Takıma gönderilecek kullanıcı girdisidir.</param>
    /// <param name="cancellationToken">İptal bildirimidir.</param>
    /// <returns>Team çalışması sırasında üretilen olay stream'idir.</returns>
    public async IAsyncEnumerable<TeamExecutionEvent> ExecuteStreamAsync(
        string teamId,
        string input,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            yield return TeamExecutionEvent.TeamFailed(
                teamId,
                "Team input cannot be empty.",
                "InputRequired");

            yield break;
        }

        var team = FindTeam(teamId);

        if (team is null)
        {
            yield return TeamExecutionEvent.TeamFailed(
                teamId,
                $"Agent team '{teamId}' was not found.",
                "TeamNotFound");

            yield break;
        }

        if (team.Members.Count == 0)
        {
            yield return TeamExecutionEvent.TeamFailed(
                team.Id,
                $"Agent team '{team.Id}' does not have any members.",
                "TeamHasNoMembers");

            yield break;
        }

        yield return TeamExecutionEvent.TeamStarted(
            team.Id,
            team.Name);

        var currentInput = BuildInitialMemberInput(team, input);
        string? finalContent = null;

        foreach (var member in team.Members)
        {
            cancellationToken.ThrowIfCancellationRequested();

            yield return TeamExecutionEvent.MemberStarted(
                team.Id,
                member.AgentId,
                member.Role);

            var memberContentBuilder = new StringBuilder();

            await foreach (var agentEvent in agentRuntime.ExecuteStreamAsync(
                               member.AgentId,
                               currentInput,
                               toolInvoker,
                               cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (agentEvent.Kind == AgentExecutionEventKind.AssistantDelta &&
                    !string.IsNullOrEmpty(agentEvent.Content))
                {
                    memberContentBuilder.Append(agentEvent.Content);

                    yield return TeamExecutionEvent.MemberDelta(
                        team.Id,
                        member.AgentId,
                        member.Role,
                        agentEvent.Content);
                }

                if (agentEvent.Kind == AgentExecutionEventKind.Failed)
                {
                    yield return TeamExecutionEvent.MemberFailed(
                        team.Id,
                        member.AgentId,
                        member.Role,
                        agentEvent.ErrorMessage ?? "Agent team member execution failed.",
                        agentEvent.ErrorCode ?? "MemberExecutionFailed");

                    yield return TeamExecutionEvent.TeamFailed(
                        team.Id,
                        $"Agent team member '{member.AgentId}' failed.",
                        "TeamMemberFailed");

                    yield break;
                }
            }

            var memberContent = memberContentBuilder.ToString();

            if (string.IsNullOrWhiteSpace(memberContent))
            {
                yield return TeamExecutionEvent.MemberFailed(
                    team.Id,
                    member.AgentId,
                    member.Role,
                    $"Agent team member '{member.AgentId}' completed without producing output.",
                    "MemberEmptyOutput");

                yield return TeamExecutionEvent.TeamFailed(
                    team.Id,
                    $"Agent team member '{member.AgentId}' completed without producing output.",
                    "TeamMemberEmptyOutput");

                yield break;
            }

            finalContent = memberContent.Trim();

            yield return TeamExecutionEvent.MemberCompleted(
                team.Id,
                member.AgentId,
                member.Role,
                finalContent);

            currentInput = BuildNextMemberInput(
                team,
                originalInput: input,
                previousMember: member,
                previousOutput: finalContent);
        }

        yield return TeamExecutionEvent.TeamCompleted(
            team.Id,
            finalContent);
    }

    private AgentTeam? FindTeam(string teamId)
    {
        return teams.FirstOrDefault(team =>
            string.Equals(team.Id, teamId, StringComparison.OrdinalIgnoreCase));
    }

    private static string BuildInitialMemberInput(
        AgentTeam team,
        string userInput)
    {
        return $"""
        You are working as part of the agent team '{team.Name}'.

        Team instructions:
        {team.Instructions}

        User request:
        {userInput}
        """;
    }

    private static string BuildNextMemberInput(
        AgentTeam team,
        string originalInput,
        TeamMember previousMember,
        string previousOutput)
    {
        return $"""
        You are working as part of the agent team '{team.Name}'.

        Team instructions:
        {team.Instructions}

        Original user request:
        {originalInput}

        Previous team member:
        Role: {previousMember.Role}
        Agent id: {previousMember.AgentId}

        Previous member output:
        {previousOutput}

        Continue from the previous member output and produce your contribution according to your role.
        """;
    }
}