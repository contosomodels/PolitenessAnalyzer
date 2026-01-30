# Contoso.AI.PolitenessAnalyzer.Tests

**Console-based test suite** for the Contoso.AI.PolitenessAnalyzer library.

> **Note**: This is a console application, not a traditional test project. It runs tests programmatically and exits with code 0 (success) or 1 (failure).

## Why Console App Instead of Test Framework?

The PolitenessAnalyzer uses Windows App SDK components that can have COM registration issues with standard test runners. Running tests through a console application:
- Avoids "class not registered" COM errors
- Provides better control over Windows App SDK initialization
- Works reliably in CI/CD environments
- Gives clearer output and exit codes

## Running the Tests

### From Visual Studio
1. Set `Contoso.AI.PolitenessAnalyzer.Tests` as the startup project
2. Press F5 or Ctrl+F5 to run
3. View test results in the console window

### From Command Line
```bash
# Build and run
dotnet run --project Contoso.AI.PolitenessAnalyzer.Tests

# Or run the compiled executable directly
cd Contoso.AI.PolitenessAnalyzer.Tests\bin\Debug\net8.0-windows10.0.19041.0\win-x64
.\Contoso.AI.PolitenessAnalyzer.Tests.exe
```

### Exit Codes
- **0**: All tests passed ?
- **1**: One or more tests failed ?

### CI/CD Integration
```yaml
# Example GitHub Actions / Azure DevOps
- name: Run Tests
  run: dotnet run --project Contoso.AI.PolitenessAnalyzer.Tests
  # Will automatically fail the build if exit code is 1
```

## Test Coverage

This test suite provides extensive coverage of the PolitenessAnalyzer functionality:

### 1. Initialization Tests (`PolitenessAnalyzerInitializationTests.cs`)
- Tests `EnsureReadyAsync()` behavior
- Validates concurrent initialization handling
- Verifies ready state transitions
- Tests `CreateAsync()` functionality

### 2. Analysis Tests (`PolitenessAnalyzerAnalysisTests.cs`)
- Tests politeness level detection for various text types
- Validates handling of empty, null, and whitespace inputs
- Tests long text and special character handling
- Verifies concurrent analysis requests
- Tests consistency of results

### 3. Disposal Tests (`PolitenessAnalyzerDisposalTests.cs`)
- Tests proper disposal behavior
- Validates multiple disposal calls
- Tests `using` and `await using` statements
- Verifies independence of multiple instances

### 4. Edge Case Tests (`PolitenessAnalyzerEdgeCaseTests.cs`)
- Single words and characters
- Special characters and emojis
- URLs, emails, and HTML tags
- Mixed case and all caps text
- Very long inputs
- Code snippets and technical content

### 5. Performance Tests (`PolitenessAnalyzerPerformanceTests.cs`)
- Response time validation
- Sequential and parallel request handling
- Inference time verification
- Stress testing (skipped by default)

### 6. Integration Tests (`PolitenessAnalyzerIntegrationTests.cs`)
- Real-world scenarios:
  - Customer service responses
  - Formal emails
  - Casual messages
  - Technical documentation
  - Complaint messages
  - Positive feedback
  - Meeting invitations
  - Apology messages

### 7. Type Tests
- `PolitenessAnalysisResponseTests.cs` - Response type validation
- `PolitenessLevelTests.cs` - Enum behavior tests

## Test Framework

This project uses a **custom console-based test runner** implemented in `Program.cs`:
- Lightweight attribute-based test discovery
- Sequential test execution (avoids COM threading issues)
- Clear console output with colored results
- Proper exit codes for CI/CD integration
- No external test framework dependencies (xUnit, NUnit, etc.)

### Custom Test Attributes
- `[Fact]` - Marks a test method
- `[Theory]` with `[InlineData]` - Parameterized tests
- `[Skip]` - Skip a test with a reason
- `IAsyncLifetime` - Test class setup/teardown

## Requirements

- .NET 8.0
- Windows 10 SDK 19041 or later
- Visual Studio 2022 or later (recommended)
- **Windows App SDK 1.8 or later** (required for COM interop with the main library)

## Important Notes

- **Console Application**: This is not a standard xUnit/NUnit test project
- **Windows App SDK Dependency**: Required for COM interop with the main library
- Model file is automatically copied from the main project during build
- Tests run sequentially to avoid COM threading issues
- Exit code indicates pass/fail for CI/CD integration
- Test output is color-coded (green=pass, red=fail, yellow=skip)

## Test Framework

- **xUnit** - Primary testing framework
- **coverlet** - Code coverage collection

## Troubleshooting

### Model file not found
- Build the main project first: `dotnet build Contoso.AI.PolitenessAnalyzer`
- The model downloads automatically and is copied to the test output directory

### Tests don't run
- Ensure you're running on Windows 10 (19041) or later
- Check that Windows App SDK is properly installed
- Try running with elevated permissions if needed

### Unexpected test failures
- Some integration tests depend on model predictions which can vary
- Check if the model file is corrupted or missing
- Try rebuilding both projects from scratch

## Contributing

When adding new tests:
1. Add test methods with `[Fact]` or `[Theory]` attributes
2. Follow the existing naming conventions (MethodName_ShouldBehavior_Condition)
3. Use descriptive test names that explain the scenario
4. The test runner will automatically discover and run new tests
5. Consider both positive and negative test cases
6. Update this README if adding new test categories
