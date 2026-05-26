using Runiq.Teams.Models.Teams;

namespace Runiq.Teams.Tests.Models.Teams;

/// <summary>
/// Takım üyesi tanımının doğrulama ve normalize etme davranışlarını doğrular.
/// </summary>
public sealed class TeamMemberTests
{
    /// <summary>
    /// Geçerli bilgilerle oluşturulan takım üyesinin alanlarını doğru taşıdığını doğrular.
    /// </summary>
    [Fact]
    public void Constructor_ShouldCreateMember_WhenValuesAreValid()
    {
        var member = new TeamMember(
            agentId: " research-agent ",
            role: " Researcher ",
            instructions: " Read source material. ");

        Assert.Equal("research-agent", member.AgentId);
        Assert.Equal("Researcher", member.Role);
        Assert.Equal("Read source material.", member.Instructions);
    }

    /// <summary>
    /// Boş yönerge verildiğinde üye yönergesinin null olarak saklandığını doğrular.
    /// </summary>
    [Fact]
    public void Constructor_ShouldSetInstructionsToNull_WhenInstructionsAreEmpty()
    {
        var member = new TeamMember(
            agentId: "research-agent",
            role: "Researcher",
            instructions: " ");

        Assert.Null(member.Instructions);
    }

    /// <summary>
    /// Agent kimliği boş verildiğinde hata fırlatıldığını doğrular.
    /// </summary>
    [Fact]
    public void Constructor_ShouldThrow_WhenAgentIdIsEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new TeamMember(
                agentId: " ",
                role: "Researcher"));

        Assert.Equal("agentId", exception.ParamName);
    }

    /// <summary>
    /// Rol adı boş verildiğinde hata fırlatıldığını doğrular.
    /// </summary>
    [Fact]
    public void Constructor_ShouldThrow_WhenRoleIsEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(() =>
            new TeamMember(
                agentId: "research-agent",
                role: " "));

        Assert.Equal("role", exception.ParamName);
    }
}