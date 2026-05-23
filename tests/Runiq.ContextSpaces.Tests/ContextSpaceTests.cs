using Runiq.ContextSpaces.Models;

namespace Runiq.ContextSpaces.Tests;

public sealed class ContextSpaceTests
{
    [Fact]
    public void Constructor_ShouldTrimIdNameAndDescription()
    {
        // ContextSpace oluşturulurken temel metin alanlarının normalize edildiğini doğrular.
        var contextSpace = new ContextSpace(
            id: " travel-planning ",
            name: " Travel Planning ",
            description: " Shared travel context ");

        Assert.Equal("travel-planning", contextSpace.Id);
        Assert.Equal("Travel Planning", contextSpace.Name);
        Assert.Equal("Shared travel context", contextSpace.Description);
    }

    [Fact]
    public void Constructor_ShouldSetDescriptionToNull_WhenDescriptionIsEmpty()
    {
        // Boş açıklama verildiğinde metadata çıktısında gereksiz boş metin taşınmadığını doğrular.
        var contextSpace = new ContextSpace(
            id: "travel-planning",
            name: "Travel Planning",
            description: " ");

        Assert.Null(contextSpace.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrow_WhenIdIsEmpty(string id)
    {
        // ContextSpace teknik kimliğinin boş bırakılamayacağını doğrular.
        var exception = Assert.Throws<ArgumentException>(() =>
            new ContextSpace(id, "Travel Planning"));

        Assert.Equal("id", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldThrow_WhenNameIsEmpty(string name)
    {
        // ContextSpace görünen adının boş bırakılamayacağını doğrular.
        var exception = Assert.Throws<ArgumentException>(() =>
            new ContextSpace("travel-planning", name));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void AddSource_ShouldAddSource()
    {
        // ContextSpace içine bilgi kaynağı eklenebildiğini doğrular.
        var contextSpace = new ContextSpace("travel-planning", "Travel Planning");

        contextSpace.AddSource(new ContextSpaceSource(
            id: "travel-docs",
            name: "Travel Documents",
            kind: ContextSpaceSourceKind.UploadedDocuments));

        var source = Assert.Single(contextSpace.Sources);
        Assert.Equal("travel-docs", source.Id);
        Assert.Equal("Travel Documents", source.Name);
        Assert.Equal(ContextSpaceSourceKind.UploadedDocuments, source.Kind);
    }

    [Fact]
    public void AddSource_ShouldThrow_WhenSourceIdAlreadyExistsIgnoringCase()
    {
        // Aynı ContextSpace içinde kaynak id tekrarının case-insensitive engellendiğini doğrular.
        var contextSpace = new ContextSpace("travel-planning", "Travel Planning");

        contextSpace.AddSource(new ContextSpaceSource(
            id: "travel-docs",
            name: "Travel Documents",
            kind: ContextSpaceSourceKind.UploadedDocuments));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            contextSpace.AddSource(new ContextSpaceSource(
                id: "TRAVEL-DOCS",
                name: "Other Travel Documents",
                kind: ContextSpaceSourceKind.UploadedDocuments)));

        Assert.Contains("travel-docs", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}