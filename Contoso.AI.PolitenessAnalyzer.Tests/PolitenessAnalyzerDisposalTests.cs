using System.Runtime.Versioning;

namespace Contoso.AI.Tests;

[SupportedOSPlatform("windows10.0.19041.0")]
public class PolitenessAnalyzerDisposalTests
{
    [Fact]
    public async Task Dispose_ShouldNotThrow_WhenCalledOnce()
    {
        // Arrange
        var analyzer = await PolitenessAnalyzer.CreateAsync();

        // Act & Assert
        var exception = Record.Exception(() => analyzer.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    public async Task Dispose_ShouldNotThrow_WhenCalledMultipleTimes()
    {
        // Arrange
        var analyzer = await PolitenessAnalyzer.CreateAsync();

        // Act & Assert
        analyzer.Dispose();
        var exception = Record.Exception(() => analyzer.Dispose());
        Assert.Null(exception);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldThrow_AfterDisposal()
    {
        // Arrange
        var analyzer = await PolitenessAnalyzer.CreateAsync();
        analyzer.Dispose();

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(
            async () => await analyzer.AnalyzeAsync("Test text"));
    }

    [Fact]
    public async Task UsingStatement_ShouldDisposeAutomatically()
    {
        // Arrange
        PolitenessAnalyzer analyzer;

        // Act
        using (analyzer = await PolitenessAnalyzer.CreateAsync())
        {
            var result = await analyzer.AnalyzeAsync("Test text");
            Assert.NotNull(result);
        }

        // Assert - analyzer should be disposed
        await Assert.ThrowsAsync<ObjectDisposedException>(
            async () => await analyzer.AnalyzeAsync("Test text"));
    }

    [Fact]
    public async Task AwaitUsingStatement_ShouldDisposeAutomatically()
    {
        // Arrange
        PolitenessAnalyzer analyzer;

        // Act
        using (analyzer = await PolitenessAnalyzer.CreateAsync())
        {
            var result = await analyzer.AnalyzeAsync("Test text");
            Assert.NotNull(result);
        }

        // Assert - analyzer should be disposed
        await Assert.ThrowsAsync<ObjectDisposedException>(
            async () => await analyzer.AnalyzeAsync("Test text"));
    }

    [Fact]
    public async Task MultipleInstances_ShouldDisposeIndependently()
    {
        // Arrange
        var analyzer1 = await PolitenessAnalyzer.CreateAsync();
        var analyzer2 = await PolitenessAnalyzer.CreateAsync();

        // Act
        analyzer1.Dispose();

        // Assert - analyzer2 should still work
        var result = await analyzer2.AnalyzeAsync("Test text");
        Assert.NotNull(result);

        // Cleanup
        analyzer2.Dispose();
    }
}
