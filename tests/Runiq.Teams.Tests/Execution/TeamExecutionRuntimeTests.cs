using Runiq.Agents;
using Runiq.Agents.Providers.OpenAI;
using Runiq.Agents.Runtime;
using Runiq.Agents.Tools;
using Runiq.Teams.Execution;
using Runiq.Teams.Models.Execution;
using Runiq.Teams.Models.Teams;
using Microsoft.Extensions.DependencyInjection;

namespace Runiq.Teams.Tests.Execution;

/// <summary>
/// Agent team runtime yürütme davranışlarını doğrular.
/// </summary>
public sealed class TeamExecutionRuntimeTests
{
    /// <summary>
    /// Takım üyesi olarak tanımlanan agent runtime'da bulunamadığında member ve team fail eventlerinin üretildiğini doğrular.
    /// </summary>
    [Fact]
    public async Task ExecuteStreamAsync_ShouldFailTeam_WhenMemberAgentDoesNotExist()
    {
        var team = new AgentTeam(
                id: "travel-team",
                name: "Travel Planning Team",
                instructions: "Create travel plans.")
            .AddMember(
                agentId: "missing-agent",
                role: "Researcher");

        var runtime = CreateRuntime(
            teams: [team],
            agents: []);

        var events = await runtime.ExecuteStreamAsync(
                teamId: "travel-team",
                input: "Create a two day travel plan.")
            .ToListAsync();

        Assert.Collection(
            events,
            first =>
            {
                Assert.Equal(TeamExecutionEventType.TeamStarted, first.Type);
                Assert.Equal("travel-team", first.TeamId);
                Assert.Equal("Travel Planning Team", first.TeamName);
            },
            second =>
            {
                Assert.Equal(TeamExecutionEventType.MemberStarted, second.Type);
                Assert.Equal("missing-agent", second.MemberAgentId);
                Assert.Equal("Researcher", second.MemberRole);
            },
            third =>
            {
                Assert.Equal(TeamExecutionEventType.MemberFailed, third.Type);
                Assert.Equal("missing-agent", third.MemberAgentId);
                Assert.Equal("Researcher", third.MemberRole);
                Assert.Equal("AgentNotFound", third.ErrorCode);
                Assert.Equal("Agent 'missing-agent' was not found.", third.ErrorMessage);
            },
            fourth =>
            {
                Assert.Equal(TeamExecutionEventType.TeamFailed, fourth.Type);
                Assert.Equal("travel-team", fourth.TeamId);
                Assert.Equal("TeamMemberFailed", fourth.ErrorCode);
                Assert.Equal("Agent team member 'missing-agent' failed.", fourth.ErrorMessage);
            });
    }

    /// <summary>
    /// Bilinmeyen team kimliği verildiğinde team not found event'i üretildiğini doğrular.
    /// </summary>
    [Fact]
    public async Task ExecuteStreamAsync_ShouldFail_WhenTeamDoesNotExist()
    {
        var runtime = CreateRuntime(
            teams: [],
            agents: []);

        var events = await runtime.ExecuteStreamAsync(
                teamId: "missing-team",
                input: "Create a plan.")
            .ToListAsync();

        var executionEvent = Assert.Single(events);

        Assert.Equal(TeamExecutionEventType.TeamFailed, executionEvent.Type);
        Assert.Equal("missing-team", executionEvent.TeamId);
        Assert.Equal("TeamNotFound", executionEvent.ErrorCode);
        Assert.Equal("Agent team 'missing-team' was not found.", executionEvent.ErrorMessage);
    }

    /// <summary>
    /// Boş kullanıcı girdisi verildiğinde input validation hatası üretildiğini doğrular.
    /// </summary>
    [Fact]
    public async Task ExecuteStreamAsync_ShouldFail_WhenInputIsEmpty()
    {
        var runtime = CreateRuntime(
            teams: [],
            agents: []);

        var events = await runtime.ExecuteStreamAsync(
                teamId: "travel-team",
                input: " ")
            .ToListAsync();

        var executionEvent = Assert.Single(events);

        Assert.Equal(TeamExecutionEventType.TeamFailed, executionEvent.Type);
        Assert.Equal("travel-team", executionEvent.TeamId);
        Assert.Equal("InputRequired", executionEvent.ErrorCode);
        Assert.Equal("Team input cannot be empty.", executionEvent.ErrorMessage);
    }

    /// <summary>
    /// Üyesi olmayan team çalıştırıldığında team has no members hatası üretildiğini doğrular.
    /// </summary>
    [Fact]
    public async Task ExecuteStreamAsync_ShouldFail_WhenTeamHasNoMembers()
    {
        var team = new AgentTeam(
            id: "empty-team",
            name: "Empty Team",
            instructions: "Does nothing.");

        var runtime = CreateRuntime(
            teams: [team],
            agents: []);

        var events = await runtime.ExecuteStreamAsync(
                teamId: "empty-team",
                input: "Create a plan.")
            .ToListAsync();

        var executionEvent = Assert.Single(events);

        Assert.Equal(TeamExecutionEventType.TeamFailed, executionEvent.Type);
        Assert.Equal("empty-team", executionEvent.TeamId);
        Assert.Equal("TeamHasNoMembers", executionEvent.ErrorCode);
        Assert.Equal("Agent team 'empty-team' does not have any members.", executionEvent.ErrorMessage);
    }

    private static TeamExecutionRuntime CreateRuntime(
        IReadOnlyList<AgentTeam> teams,
        IReadOnlyList<Agent> agents)
    {

        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var toolInvoker = new AgentToolInvoker(serviceProvider);

        var agentRuntime = new AgentExecutionRuntime(
            agents,
            CreateOpenAIResponsesClient(),
            CreateOpenAICompatibleClient(),
            toolInvoker);

        return new TeamExecutionRuntime(
            teams,
            agentRuntime,
            toolInvoker);
    }

    private static OpenAIResponsesClient CreateOpenAIResponsesClient()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://example.test")
        };

        return new OpenAIResponsesClient(httpClient);
    }

    private static OpenAICompatibleClient CreateOpenAICompatibleClient()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://example.test")
        };

        return new OpenAICompatibleClient(httpClient);
    }
}