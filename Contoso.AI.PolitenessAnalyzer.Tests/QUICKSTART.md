# Quick Start Guide - Running Tests

## TL;DR

```bash
# Clone and build
git clone https://github.com/contosomodels/PolitenessAnalyzer
cd PolitenessAnalyzer

# Build main project (downloads model)
dotnet build Contoso.AI.PolitenessAnalyzer

# Run tests
dotnet run --project Contoso.AI.PolitenessAnalyzer.Tests

# Check results
# Exit code 0 = All tests passed ?
# Exit code 1 = Some tests failed ?
```

## What You'll See

```
===================================================
Contoso.AI.PolitenessAnalyzer Test Suite
===================================================

Running PolitenessLevel Tests...
--------------------------------------------------
  ? PolitenessLevel_ShouldHaveExpectedValues
  ? PolitenessLevel_ShouldConvertToString
  ...

Running Analysis Tests...
--------------------------------------------------
  ? AnalyzeAsync_ShouldHandleEmptyString
  ? AnalyzeAsync_ShouldHandleNull
  ...

===================================================
Test Results Summary
===================================================
Total Tests:   68
Passed:        68 ?
Failed:        0 ?
Skipped:       1 ?
Duration:      45.23s
===================================================

? ALL TESTS PASSED
```

## Why Console App?

Traditional test frameworks (xUnit, NUnit) can have issues with Windows App SDK COM initialization. This console-based approach:
- ? No "class not registered" errors
- ? Works everywhere (VS, CLI, CI/CD)
- ? Clear exit codes
- ? Colored output

## Test Statistics

- **68+ test cases** across 8 test classes
- **~45 seconds** runtime (includes model inference)
- **Exit code 0** = success, **1** = failure
- **Color-coded** console output

## Requirements

- Windows 10 (19041) or later
- .NET 8 SDK
- ~100MB disk space (for model file)

## CI/CD Integration

### GitHub Actions
```yaml
- name: Run Tests
  run: dotnet run --project Contoso.AI.PolitenessAnalyzer.Tests
```

### Azure Pipelines
```yaml
- script: dotnet run --project Contoso.AI.PolitenessAnalyzer.Tests
  displayName: 'Run Tests'
```

The build will automatically fail if tests fail (exit code 1).

## Test Categories

? **Initialization** - Startup and resource management  
? **Analysis** - Core politeness detection  
? **Edge Cases** - Special characters, emojis, long text  
? **Performance** - Response time validation  
? **Integration** - Real-world scenarios  
? **Disposal** - Resource cleanup  
? **Type Tests** - Enum and response types  

## Troubleshooting

**"Model file not found"**  
? Run `dotnet build Contoso.AI.PolitenessAnalyzer` first

**"Platform not supported"**  
? Requires Windows 10 (19041) or later

**Tests crash or hang**  
? Check Windows App SDK is installed properly

---

For more details, see [README.md](README.md) and [WINDOWS_APP_SDK_TESTING.md](WINDOWS_APP_SDK_TESTING.md).
