using System.Runtime.Versioning;

namespace Contoso.AI.Tests;

[SupportedOSPlatform("windows10.0.19041.0")]
public class PolitenessAnalyzerPerformanceTests : IAsyncLifetime
{
    private PolitenessAnalyzer? _analyzer;

    public async Task InitializeAsync()
    {
        _analyzer = await PolitenessAnalyzer.CreateAsync();
    }

    public Task DisposeAsync()
    {
        _analyzer?.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldCompleteWithinReasonableTime()
    {
        // Arrange
        var text = "Thank you for your assistance with this matter.";
        var maxExpectedMs = 5000; // 5 seconds should be more than enough

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _analyzer!.AnalyzeAsync(text);
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < maxExpectedMs,
            $"Analysis took {stopwatch.ElapsedMilliseconds}ms, expected less than {maxExpectedMs}ms");
        Assert.NotNull(result);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleMultipleSequentialCalls()
    {
        // Arrange
        var texts = Enumerable.Range(0, 10)
            .Select(i => $"Test message number {i}. Thank you!")
            .ToList();

        // Act
        var results = new List<PolitenessAnalysisResponse>();
        foreach (var text in texts)
        {
            results.Add(await _analyzer!.AnalyzeAsync(text));
        }

        // Assert
        Assert.Equal(10, results.Count);
        Assert.All(results, result => Assert.NotNull(result));
    }

    [Fact]
    public async Task AnalyzeAsync_InferenceTimeShouldBeRealistic()
    {
        // Arrange
        var text = "Thank you for your help.";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.True(result.InferenceTimeMs >= 0, "Inference time should be non-negative");
        Assert.True(result.InferenceTimeMs < 10000, "Inference time should be less than 10 seconds");
    }

    [Fact(Skip = "Long running test - enable manually for performance testing")]
    public async Task AnalyzeAsync_StressTest_MultipleParallelRequests()
    {
        // Arrange
        var requestCount = 100;
        var texts = Enumerable.Range(0, requestCount)
            .Select(i => $"Test message {i}. Thank you for your time.")
            .ToList();

        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var tasks = texts.Select(text => _analyzer!.AnalyzeAsync(text));
        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        // Assert
        Assert.Equal(requestCount, results.Length);
        Assert.All(results, result => Assert.NotNull(result));
        
        var avgTimePerRequest = stopwatch.ElapsedMilliseconds / requestCount;
        Assert.True(avgTimePerRequest < 1000, 
            $"Average time per request was {avgTimePerRequest}ms, expected less than 1000ms");
    }

    [Fact]
    public async Task CreateAsync_ShouldCompleteQuickly_AfterFirstInitialization()
    {
        // Arrange - ensure already initialized
        await PolitenessAnalyzer.EnsureReadyAsync();
        
        // Act
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var analyzer = await PolitenessAnalyzer.CreateAsync();
        stopwatch.Stop();

        // Assert
        Assert.True(stopwatch.ElapsedMilliseconds < 3000,
            $"CreateAsync took {stopwatch.ElapsedMilliseconds}ms after initialization");
        
        analyzer.Dispose();
    }
}
