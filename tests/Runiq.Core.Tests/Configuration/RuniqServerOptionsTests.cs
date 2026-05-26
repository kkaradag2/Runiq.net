using Runiq.ContextSpaces.Models.Sources;
using Runiq.Core.Configuration;
using Runiq.Teams.Models.Teams;

namespace Runiq.Core.Tests.Configuration;

public sealed class RuniqServerOptionsTests
{
    [Fact]
    public void AddContextSpace_ShouldRegisterContextSpace()
    {
        // RuniqServerOptions içine ContextSpace kaydının eklenebildiğini doğrular.
        var options = new RuniqServerOptions();

        options.AddContextSpace(new ContextSpace(
            id: "travel-planning",
            name: "Travel Planning Context"));

        var contextSpace = Assert.Single(options.ContextSpaces);

        Assert.Equal("travel-planning", contextSpace.Id);
        Assert.Equal("Travel Planning Context", contextSpace.Name);
    }

    [Fact]
    public void AddContextSpace_ShouldThrow_WhenContextSpaceIdAlreadyExistsIgnoringCase()
    {
        // Aynı ContextSpace id değerinin case-insensitive olarak ikinci kez eklenemeyeceğini doğrular.
        var options = new RuniqServerOptions();

        options.AddContextSpace(new ContextSpace(
            id: "travel-planning",
            name: "Travel Planning Context"));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            options.AddContextSpace(new ContextSpace(
                id: "TRAVEL-PLANNING",
                name: "Duplicate Travel Planning Context")));

        Assert.Contains("travel-planning", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AddContextSpace_ShouldReturnSameOptionsInstance()
    {
        // AddContextSpace metodunun fluent configuration için aynı options örneğini döndürdüğünü doğrular.
        var options = new RuniqServerOptions();

        var result = options.AddContextSpace(new ContextSpace(
            id: "travel-planning",
            name: "Travel Planning Context"));

        Assert.Same(options, result);
    }

    /// <summary>
    /// Geçerli bir agent team kaydının runtime seçeneklerine eklenebildiğini doğrular.
    /// </summary>
    [Fact]
    public void AddTeam_ShouldRegisterTeam_WhenTeamIsValid()
    {
        var options = new RuniqServerOptions();

        var team = new AgentTeam(
            id: "travel-team",
            name: "Travel Planning Team",
            instructions: "Create travel plans with multiple specialized agents.");

        var returnedOptions = options.AddTeam(team);

        Assert.Same(options, returnedOptions);
        Assert.Single(options.Teams);
        Assert.Same(team, options.Teams[0]);
    }

    /// <summary>
    /// Aynı kimliğe sahip ikinci bir agent team kaydı eklendiğinde hata fırlatıldığını doğrular.
    /// </summary>
    [Fact]
    public void AddTeam_ShouldThrow_WhenTeamIdAlreadyExists()
    {
        var options = new RuniqServerOptions();

        options.AddTeam(new AgentTeam(
            id: "travel-team",
            name: "Travel Planning Team",
            instructions: "Create travel plans."));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            options.AddTeam(new AgentTeam(
                id: "TRAVEL-TEAM",
                name: "Another Travel Team",
                instructions: "Create another travel plan.")));

        Assert.Equal(
            "An agent team with id 'TRAVEL-TEAM' is already registered.",
            exception.Message);
    }

    /// <summary>
    /// Null agent team kaydı eklendiğinde hata fırlatıldığını doğrular.
    /// </summary>
    [Fact]
    public void AddTeam_ShouldThrow_WhenTeamIsNull()
    {
        var options = new RuniqServerOptions();

        Assert.Throws<ArgumentNullException>(() => options.AddTeam(null!));
    }

}