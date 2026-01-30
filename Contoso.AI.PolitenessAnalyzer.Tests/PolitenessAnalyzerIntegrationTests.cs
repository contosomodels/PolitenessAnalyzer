using System.Runtime.Versioning;

namespace Contoso.AI.Tests;

/// <summary>
/// Integration tests that verify real-world scenarios for the PolitenessAnalyzer.
/// These tests use the actual model and verify end-to-end behavior.
/// </summary>
[SupportedOSPlatform("windows10.0.19041.0")]
public class PolitenessAnalyzerIntegrationTests : IAsyncLifetime
{
    private PolitenessAnalyzer? _analyzer;

    public async Task InitializeAsync()
    {
        // Ensure the analyzer is fully initialized before running tests
        var result = await PolitenessAnalyzer.EnsureReadyAsync();
        Assert.Equal(AIFeatureReadyResultState.Success, result.Status);
        
        _analyzer = await PolitenessAnalyzer.CreateAsync();
    }

    public Task DisposeAsync()
    {
        _analyzer?.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task RealWorldScenario_CustomerServiceResponse()
    {
        // Arrange
        var politeResponse = "Thank you for reaching out to us! We're sorry to hear about " +
                           "the issue you're experiencing. We'd be happy to help you resolve this. " +
                           "Could you please provide more details?";

        // Act
        var result = await _analyzer!.AnalyzeAsync(politeResponse);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
        Assert.Contains("polite", result.Description.ToLower());
    }

    [Fact]
    public async Task RealWorldScenario_FormEmail()
    {
        // Arrange
        var formalEmail = "Dear Sir/Madam, I hope this message finds you well. " +
                         "I am writing to inquire about the status of my application. " +
                         "I would greatly appreciate any updates you can provide. " +
                         "Thank you for your time and consideration. Best regards.";

        // Act
        var result = await _analyzer!.AnalyzeAsync(formalEmail);

        // Assert
        Assert.NotNull(result);
        Assert.True(
            result.Level == PolitenessLevel.Polite || result.Level == PolitenessLevel.SomewhatPolite,
            $"Expected polite level for formal email, got {result.Level}");
    }

    [Fact]
    public async Task RealWorldScenario_CasualMessage()
    {
        // Arrange
        var casualMessage = "Hey, can you send me the file? Thanks.";

        // Act
        var result = await _analyzer!.AnalyzeAsync(casualMessage);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task RealWorldScenario_TechnicalDocumentation()
    {
        // Arrange
        var technicalText = "The function accepts two parameters: input and output. " +
                          "It returns a boolean value indicating success or failure. " +
                          "See documentation for more details.";

        // Act
        var result = await _analyzer!.AnalyzeAsync(technicalText);

        // Assert
        Assert.NotNull(result);
        Assert.True(
            result.Level == PolitenessLevel.Neutral || 
            result.Level == PolitenessLevel.SomewhatPolite,
            $"Expected neutral or somewhat polite for technical text, got {result.Level}");
    }

    [Fact]
    public async Task RealWorldScenario_ComplaintMessage()
    {
        // Arrange
        var complaint = "This is unacceptable. I've been waiting for hours and nobody has helped me. " +
                       "This is the worst service I've ever experienced.";

        // Act
        var result = await _analyzer!.AnalyzeAsync(complaint);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
        // Note: The actual level may vary based on model interpretation
    }

    [Fact]
    public async Task RealWorldScenario_PositiveFeedback()
    {
        // Arrange
        var feedback = "I wanted to express my sincere gratitude for your exceptional service. " +
                      "Your team went above and beyond to help me. Thank you so much!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(feedback);

        // Assert
        Assert.NotNull(result);
        Assert.True(
            result.Level == PolitenessLevel.Polite || result.Level == PolitenessLevel.SomewhatPolite,
            $"Expected polite level for positive feedback, got {result.Level}");
    }

    [Fact]
    public async Task RealWorldScenario_QuestionWithPleaseAndThankYou()
    {
        // Arrange
        var question = "Could you please help me understand how this works? Thank you in advance!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(question);

        // Assert
        Assert.NotNull(result);
        Assert.True(
            result.Level == PolitenessLevel.Polite || result.Level == PolitenessLevel.SomewhatPolite,
            $"Expected polite level for question with 'please' and 'thank you', got {result.Level}");
    }

    [Fact]
    public async Task RealWorldScenario_DirectInstruction()
    {
        // Arrange
        var instruction = "Complete the task by end of day. Submit the report to management.";

        // Act
        var result = await _analyzer!.AnalyzeAsync(instruction);

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result.Description);
    }

    [Fact]
    public async Task RealWorldScenario_MeetingInvitation()
    {
        // Arrange
        var invitation = "I hope you're doing well! I'd like to invite you to a meeting " +
                        "to discuss the project. Would next Tuesday work for you? " +
                        "Please let me know. Thanks!";

        // Act
        var result = await _analyzer!.AnalyzeAsync(invitation);

        // Assert
        Assert.NotNull(result);
        Assert.True(
            result.Level == PolitenessLevel.Polite || 
            result.Level == PolitenessLevel.SomewhatPolite ||
            result.Level == PolitenessLevel.Neutral,
            $"Expected polite, somewhat polite, or neutral for meeting invitation, got {result.Level}");
    }

    [Fact]
    public async Task RealWorldScenario_ApologyMessage()
    {
        // Arrange
        var apology = "I sincerely apologize for the inconvenience this has caused. " +
                     "We are working hard to resolve the issue. Thank you for your patience " +
                     "and understanding.";

        // Act
        var result = await _analyzer!.AnalyzeAsync(apology);

        // Assert
        Assert.NotNull(result);
        Assert.True(
            result.Level == PolitenessLevel.Polite || result.Level == PolitenessLevel.SomewhatPolite,
            $"Expected polite level for apology, got {result.Level}");
    }
}
