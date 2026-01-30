using System.Runtime.Versioning;

namespace Contoso.AI.Tests;

[SupportedOSPlatform("windows10.0.19041.0")]
public class PolitenessAnalyzerEdgeCaseTests : IAsyncLifetime
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
    public async Task AnalyzeAsync_ShouldHandleSingleWord()
    {
        // Act
        var result = await _analyzer!.AnalyzeAsync("Thanks");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleSingleCharacter()
    {
        // Act
        var result = await _analyzer!.AnalyzeAsync("A");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleRepeatedCharacters()
    {
        // Act
        var result = await _analyzer!.AnalyzeAsync("!!!!!!");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleNumbersOnly()
    {
        // Act
        var result = await _analyzer!.AnalyzeAsync("123456789");

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleNewlines()
    {
        // Arrange
        var text = "Thank you\nfor your\nhelp";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleTabs()
    {
        // Arrange
        var text = "Thank\tyou\tfor\tyour\thelp";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleEmojis()
    {
        // Arrange
        var text = "Thank you so much! ??????";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleMixedCase()
    {
        // Arrange
        var text = "ThAnK YoU fOr YoUr HeLp!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleAllCaps()
    {
        // Arrange
        var text = "THANK YOU FOR YOUR HELP!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleUrlsInText()
    {
        // Arrange
        var text = "Please check https://example.com for more information. Thank you!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleEmailAddresses()
    {
        // Arrange
        var text = "Please contact support@example.com for assistance. Thank you!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleHtmlTags()
    {
        // Arrange
        var text = "<p>Thank you for your help!</p>";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleCodeSnippets()
    {
        // Arrange
        var text = "Please use var x = 10; in your code. Thank you!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleVeryLongSingleWord()
    {
        // Arrange
        var text = new string('a', 1000);

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task AnalyzeAsync_ShouldHandleMultipleSentences()
    {
        // Arrange
        var text = "Hello. Thank you. I appreciate your help. Have a great day!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(text);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }
}
