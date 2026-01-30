namespace Contoso.AI.Tests;

public class PolitenessLevelTests
{
    [Fact]
    public void PolitenessLevel_ShouldHaveExpectedValues()
    {
        // Assert
        Assert.Equal(0, (int)PolitenessLevel.Polite);
        Assert.Equal(1, (int)PolitenessLevel.SomewhatPolite);
        Assert.Equal(2, (int)PolitenessLevel.Neutral);
        Assert.Equal(3, (int)PolitenessLevel.Impolite);
    }

    [Fact]
    public void PolitenessLevel_ShouldConvertToString()
    {
        // Act & Assert
        Assert.Equal("Polite", PolitenessLevel.Polite.ToString());
        Assert.Equal("SomewhatPolite", PolitenessLevel.SomewhatPolite.ToString());
        Assert.Equal("Neutral", PolitenessLevel.Neutral.ToString());
        Assert.Equal("Impolite", PolitenessLevel.Impolite.ToString());
    }

    [Fact]
    public void PolitenessLevel_ShouldParseFromString()
    {
        // Act & Assert
        Assert.Equal(PolitenessLevel.Polite, Enum.Parse<PolitenessLevel>("Polite"));
        Assert.Equal(PolitenessLevel.SomewhatPolite, Enum.Parse<PolitenessLevel>("SomewhatPolite"));
        Assert.Equal(PolitenessLevel.Neutral, Enum.Parse<PolitenessLevel>("Neutral"));
        Assert.Equal(PolitenessLevel.Impolite, Enum.Parse<PolitenessLevel>("Impolite"));
    }

    [Fact]
    public void PolitenessLevel_ShouldSupportComparison()
    {
        // Assert
        Assert.True(PolitenessLevel.Polite == PolitenessLevel.Polite);
        Assert.True(PolitenessLevel.Polite != PolitenessLevel.Impolite);
        Assert.False(PolitenessLevel.Neutral == PolitenessLevel.SomewhatPolite);
    }

    [Fact]
    public void PolitenessLevel_ShouldBeUsableInSwitch()
    {
        // Arrange
        var level = PolitenessLevel.Polite;

        // Act
        var result = level switch
        {
            PolitenessLevel.Polite => "Polite case",
            PolitenessLevel.SomewhatPolite => "SomewhatPolite case",
            PolitenessLevel.Neutral => "Neutral case",
            PolitenessLevel.Impolite => "Impolite case",
            _ => "Unknown"
        };

        // Assert
        Assert.Equal("Polite case", result);
    }
}
