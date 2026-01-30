# Windows App SDK Testing Guide - Console App Approach

## Architecture Decision

This test project is implemented as a **console application** rather than a traditional test project (xUnit/NUnit). This design decision solves critical issues with Windows App SDK testing.

## Why Console App?

### The Problem
The `Contoso.AI.PolitenessAnalyzer` library uses Windows App SDK components:
- `Microsoft.Windows.AI.MachineLearning` for AI inference
- ONNX Runtime execution providers requiring Windows COM components
- Traditional test runners (xUnit, NUnit, MSTest) can encounter "class not registered" errors due to COM initialization timing

### The Solution
Running tests through a console application:
- ? Full control over COM initialization order
- ? No test runner interference with Windows App SDK
- ? Works reliably in all environments (Visual Studio, command line, CI/CD)
- ? Clear exit codes (0=pass, 1=fail)
- ? Better debugging experience
- ? Colored console output

## Implementation Details

### Custom Test Framework (`Program.cs`)
The project includes a lightweight test framework:
- **Test Discovery**: Reflection-based discovery of `[Fact]` and `[Theory]` methods
- **Test Execution**: Sequential execution to avoid COM threading issues
- **Lifecycle Management**: Supports `IAsyncLifetime` for setup/teardown
- **Assertions**: Custom `Assert` class compatible with xUnit syntax
- **Exit Codes**: Proper process exit codes for CI/CD integration

## Key Configuration

### 1. Console Application Setup
```xml
<OutputType>Exe</OutputType>
<TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>
```

### 2. Windows App SDK Reference
```xml
<PackageReference Include="Microsoft.WindowsAppSDK" Version="1.8.260101001" />
```

### 3. Runtime Identifier
```xml
<RuntimeIdentifier Condition="'$(RuntimeIdentifier)' == ''">win-x64</RuntimeIdentifier>
<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>
```

### 4. Model File Copy Target
Automatically copies the model file from main project:
```xml
<Target Name="CopyModelFile" AfterTargets="Build">
  <!-- Copies from ..\Contoso.AI.PolitenessAnalyzer\obj\Models\ -->
</Target>
```

## Common Errors and Solutions

### Error: "WindowsAppSDKSelfContained requires a supported Windows architecture"
**Cause**: Missing runtime identifier  
**Solution**: Project file sets `<RuntimeIdentifier>win-x64</RuntimeIdentifier>`

### Error: "Model file not found"
**Cause**: Model not downloaded or copied  
**Solution**: Build main project first: `dotnet build Contoso.AI.PolitenessAnalyzer`

### Tests don't run or crash immediately
**Cause**: Windows App SDK not initialized properly  
**Solution**: Console app architecture handles this automatically

## Running Tests Successfully

### From Visual Studio
1. Set `Contoso.AI.PolitenessAnalyzer.Tests` as startup project
2. Press F5 or Ctrl+F5
3. View colored results in console

### From Command Line
```bash
# Build and run in one command
dotnet run --project Contoso.AI.PolitenessAnalyzer.Tests

# Or build first, then run executable
dotnet build
cd Contoso.AI.PolitenessAnalyzer.Tests\bin\Debug\net8.0-windows10.0.19041.0\win-x64
.\Contoso.AI.PolitenessAnalyzer.Tests.exe

# Check exit code (PowerShell)
echo $LASTEXITCODE  # 0 = pass, 1 = fail
```

### For CI/CD
```yaml
# GitHub Actions / Azure Pipelines
steps:
  - name: Build Main Project
    run: dotnet build Contoso.AI.PolitenessAnalyzer
  
  - name: Run Tests
    run: dotnet run --project Contoso.AI.PolitenessAnalyzer.Tests
    # Automatically fails build if exit code is 1
```

## Adding New Tests

1. Create a test class in the `Contoso.AI.Tests` namespace
2. Add methods with `[Fact]` or `[Theory]` attributes:
   ```csharp
   [Fact]
   public async Task MyNewTest()
   {
       // Arrange
       var analyzer = await PolitenessAnalyzer.CreateAsync();
       
       // Act
       var result = await analyzer.AnalyzeAsync("test text");
       
       // Assert
       Assert.NotNull(result);
       
       // Cleanup
       analyzer.Dispose();
   }
   ```
3. The test runner automatically discovers and runs new tests
4. No need to register tests or modify Program.cs
