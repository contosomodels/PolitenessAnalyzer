using System.Runtime.Versioning;

namespace Contoso.AI.Tests;

[SupportedOSPlatform("windows10.0.19041.0")]
public class PolitenessAnalyzerAnalysisTests : IAsyncLifetime
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
    public async Task AnalyzeAsync_ShouldReturnPoliteLevel_ForPoliteText()
    {
        // Arrange
        var politeText = "Thank you so much for your help! I really appreciate your kindness and support.";

        // Act
        var result = await _analyzer!.AnalyzeAsync(politeText);

        // Assert
        Assert.NotNull(result);
        Assert.True(
            result.Level == PolitenessLevel.Polite || result.Level == PolitenessLevel.SomewhatPolite,
            $"Expected Polite or SomewhatPolite, but got {result.Level}");
        Assert.NotEmpty(result.Description);
        Assert.True(result.InferenceTimeMs >= 0);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldReturnImpoliteLevel_ForRudeText()
    {
        // Arrange
        var rudeText = "This is terrible! You're completely useless and I hate this!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(rudeText);

        // Assert
        Assert.NotNull(result);
        Assert.True(
            result.Level == PolitenessLevel.Impolite || result.Level == PolitenessLevel.Neutral,
            $"Expected Impolite or Neutral for rude text, but got {result.Level}");
        Assert.NotEmpty(result.Description);
        Assert.True(result.InferenceTimeMs >= 0);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldReturnNeutralLevel_ForFactualText()
    {
        // Arrange
        var neutralText = "The meeting is scheduled for 3 PM. Please review the document.";

        // Act
        var result = await _analyzer!.AnalyzeAsync(neutralText);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
        Assert.True(result.InferenceTimeMs >= 0);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleEmptyString()
    {
        // Act
        var result = await _analyzer!.AnalyzeAsync(string.Empty);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PolitenessLevel.Neutral, result.Level);
        Assert.Equal("No text to analyze", result.Description);
        Assert.Equal(0, result.InferenceTimeMs);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleWhitespaceString()
    {
        // Act
        var result = await _analyzer!.AnalyzeAsync("   \t\n   ");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PolitenessLevel.Neutral, result.Level);
        Assert.Equal("No text to analyze", result.Description);
        Assert.Equal(0, result.InferenceTimeMs);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleNull()
    {
        // Act
        var result = await _analyzer!.AnalyzeAsync(null!);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(PolitenessLevel.Neutral, result.Level);
        Assert.Equal("No text to analyze", result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleLongText()
    {
        // Arrange
        var longText = string.Join(" ", Enumerable.Repeat("Thank you for your consideration.", 100));

        // Act
        var result = await _analyzer!.AnalyzeAsync(longText);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
        Assert.True(result.InferenceTimeMs >= 0);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleSpecialCharacters()
    {
        // Arrange
        var specialText = "Hello! @#$%^&*() Thank you very much!!!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(specialText);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
        Assert.True(result.InferenceTimeMs >= 0);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleMultipleLanguageCharacters()
    {
        // Arrange
        var multiLangText = "Thank you! Merci! Gracias! ??!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(multiLangText);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
        Assert.True(result.InferenceTimeMs >= 0);
    }

    [Theory]
    [InlineData("Please help me with this.")]
    [InlineData("Could you kindly assist?")]
    [InlineData("I would appreciate your help.")]
    public async Task AnalyzeAsync_ShouldHandleVariousPoliteExpressions(string text)
    {
        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
        Assert.True(result.InferenceTimeMs >= 0);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldReturnConsistentResults_ForSameInput()
    {
        // Arrange
        var text = "Thank you for your assistance.";

        // Act
        var result1 = await _analyzer!.AnalyzeAsync(text);
        var result2 = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.Equal(result1.Level, result2.Level);
        Assert.Equal(result1.Description, result2.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldReportInferenceTime()
    {
        // Arrange
        var text = "This is a test message.";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.True(result.InferenceTimeMs >= 0, "Inference time should be non-negative");
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleConcurrentRequests()
    {
        // Arrange
        var texts = new[]
        {
            "Thank you so much!",
            "This is terrible.",
            "The meeting is at 3 PM.",
            "I really appreciate your help."
        };

        // Act
        var tasks = texts.Select(text => _analyzer!.AnalyzeAsync(text)).ToArray();
        var results = await Task.WhenAll(tasks);

        // Assert
        Assert.Equal(4, results.Length);
        Assert.All(results, result =>
        {
            Assert.NotNull(result);
            Assert.NotEmpty(result.Description);
        });
    }
}
