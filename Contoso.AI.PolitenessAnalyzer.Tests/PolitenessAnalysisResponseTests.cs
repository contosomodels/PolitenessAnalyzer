using System.Runtime.Versioning;

namespace Contoso.AI.Tests;

[SupportedOSPlatform("windows10.0.19041.0")]
public class PolitenessAnalysisResponseTests
{
    [Fact]
    public void PolitenessAnalysisResponse_ShouldInitializeWithDefaultValues()
    {
        // Act
        var response = new PolitenessAnalysisResponse();

        // Assert
        Assert.Equal(default(PolitenessLevel), response.Level);
        Assert.Equal(string.Empty, response.Description);
        Assert.Equal(0, response.InferenceTimeMs);
    }

    [Fact]
    public void PolitenessAnalysisResponse_ShouldAcceptInitializers()
    {
        // Act
        var response = new PolitenessAnalysisResponse
        {
            Level = PolitenessLevel.Polite,
            Description = "Test description",
            InferenceTimeMs = 100
        };

        // Assert
        Assert.Equal(PolitenessLevel.Polite, response.Level);
        Assert.Equal("Test description", response.Description);
        Assert.Equal(100, response.InferenceTimeMs);
    }

    [Theory]
    [InlineData(PolitenessLevel.Polite)]
    [InlineData(PolitenessLevel.SomewhatPolite)]
    [InlineData(PolitenessLevel.Neutral)]
    [InlineData(PolitenessLevel.Impolite)]
    public void PolitenessAnalysisResponse_ShouldAcceptAllPolitenessLevels(PolitenessLevel level)
    {
        // Act
        var response = new PolitenessAnalysisResponse
        {
            Level = level,
            Description = "Test",
            InferenceTimeMs = 0
        };

        // Assert
        Assert.Equal(level, response.Level);
    }
}
