using System.Diagnostics;
using System.Reflection;
using System.Runtime.Versioning;

namespace Contoso.AI.Tests;

[SupportedOSPlatform("windows10.0.19041.0")]
class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("===================================================");
        Console.WriteLine("Contoso.AI.PolitenessAnalyzer Test Suite");
        Console.WriteLine("===================================================");
        Console.WriteLine();

        var testRunner = new TestRunner();
        var results = await testRunner.RunAllTestsAsync();

        Console.WriteLine();
        Console.WriteLine("===================================================");
        Console.WriteLine("Test Results Summary");
        Console.WriteLine("===================================================");
        Console.WriteLine($"Total Tests:   {results.TotalTests}");
        Console.WriteLine($"Passed:        {results.PassedTests} ?");
        Console.WriteLine($"Failed:        {results.FailedTests} ?");
        Console.WriteLine($"Skipped:       {results.SkippedTests} ?");
        Console.WriteLine($"Duration:      {results.TotalDuration.TotalSeconds:F2}s");
        Console.WriteLine("===================================================");
        Console.WriteLine();

        if (results.FailedTests > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("? TESTS FAILED");
            Console.ResetColor();
            return 1; // Exit code 1 for failure
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("? ALL TESTS PASSED");
        Console.ResetColor();
        return 0; // Exit code 0 for success
    }
}

class TestResults
{
    public int TotalTests { get; set; }
    public int PassedTests { get; set; }
    public int FailedTests { get; set; }
    public int SkippedTests { get; set; }
    public TimeSpan TotalDuration { get; set; }
}

[SupportedOSPlatform("windows10.0.19041.0")]
class TestRunner
{
    private int _totalTests = 0;
    private int _passedTests = 0;
    private int _failedTests = 0;
    private int _skippedTests = 0;

    public async Task<TestResults> RunAllTestsAsync()
    {
        var stopwatch = Stopwatch.StartNew();

        // Run all test classes
        await RunTestClass<PolitenessLevelTests>("PolitenessLevel Tests");
        await RunTestClass<PolitenessAnalysisResponseTests>("PolitenessAnalysisResponse Tests");
        await RunTestClass<PolitenessAnalyzerInitializationTests>("Initialization Tests");
        await RunTestClass<PolitenessAnalyzerAnalysisTests>("Analysis Tests");
        await RunTestClass<PolitenessAnalyzerDisposalTests>("Disposal Tests");
        await RunTestClass<PolitenessAnalyzerEdgeCaseTests>("Edge Case Tests");
        await RunTestClass<PolitenessAnalyzerPerformanceTests>("Performance Tests");
        await RunTestClass<PolitenessAnalyzerIntegrationTests>("Integration Tests");

        stopwatch.Stop();

        return new TestResults
        {
            TotalTests = _totalTests,
            PassedTests = _passedTests,
            FailedTests = _failedTests,
            SkippedTests = _skippedTests,
            TotalDuration = stopwatch.Elapsed
        };
    }

    private async Task RunTestClass<T>(string className) where T : new()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Running {className}...");
        Console.ResetColor();
        Console.WriteLine(new string('-', 50));

        var testClass = new T();
        var type = typeof(T);

        // Check if the class implements IAsyncLifetime
        var isAsyncLifetime = testClass is IAsyncLifetime;

        // Initialize if needed
        if (isAsyncLifetime)
        {
            await ((IAsyncLifetime)testClass).InitializeAsync();
        }

        try
        {
            // Get all public methods
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                // Check for [Fact] attribute
                var factAttribute = method.GetCustomAttribute<FactAttribute>();
                if (factAttribute != null)
                {
                    await RunTestMethod(testClass, method, factAttribute);
                    continue;
                }

                // Check for [Theory] attribute
                var theoryAttribute = method.GetCustomAttribute<TheoryAttribute>();
                if (theoryAttribute != null)
                {
                    await RunTheoryMethod(testClass, method);
                }
            }
        }
        finally
        {
            // Dispose if needed
            if (isAsyncLifetime)
            {
                await ((IAsyncLifetime)testClass).DisposeAsync();
            }
        }
    }

    private async Task RunTestMethod(object testClass, MethodInfo method, FactAttribute attribute)
    {
        _totalTests++;

        // Check if test should be skipped
        if (!string.IsNullOrEmpty(attribute.Skip))
        {
            _skippedTests++;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"  ? {method.Name} - SKIPPED: {attribute.Skip}");
            Console.ResetColor();
            return;
        }

        try
        {
            var result = method.Invoke(testClass, null);
            
            if (result is Task task)
            {
                await task;
            }

            _passedTests++;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  ? {method.Name}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            _failedTests++;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"  ? {method.Name}");
            Console.WriteLine($"    Error: {GetExceptionMessage(ex)}");
            Console.ResetColor();
        }
    }

    private async Task RunTheoryMethod(object testClass, MethodInfo method)
    {
        var inlineDataAttributes = method.GetCustomAttributes<InlineDataAttribute>();

        foreach (var inlineData in inlineDataAttributes)
        {
            _totalTests++;

            try
            {
                var result = method.Invoke(testClass, inlineData.Data);
                
                if (result is Task task)
                {
                    await task;
                }

                _passedTests++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  ? {method.Name}({string.Join(", ", inlineData.Data.Select(d => $"\"{d}\""))})");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                _failedTests++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"  ? {method.Name}({string.Join(", ", inlineData.Data.Select(d => $"\"{d}\""))})");
                Console.WriteLine($"    Error: {GetExceptionMessage(ex)}");
                Console.ResetColor();
            }
        }
    }

    private string GetExceptionMessage(Exception ex)
    {
        // Unwrap TargetInvocationException
        if (ex is TargetInvocationException tie && tie.InnerException != null)
        {
            ex = tie.InnerException;
        }

        return ex.Message;
    }
}

// Minimal xUnit attribute implementations for compatibility
[AttributeUsage(AttributeTargets.Method)]
class FactAttribute : Attribute
{
    public string? Skip { get; set; }
}

[AttributeUsage(AttributeTargets.Method)]
class TheoryAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
class InlineDataAttribute : Attribute
{
    public object[] Data { get; }

    public InlineDataAttribute(params object[] data)
    {
        Data = data;
    }
}

interface IAsyncLifetime
{
    Task InitializeAsync();
    Task DisposeAsync();
}

// Minimal Assert class for compatibility
static class Assert
{
    public static void True(bool condition, string? message = null)
    {
        if (!condition)
            throw new AssertionException(message ?? "Expected true but got false");
    }

    public static void False(bool condition, string? message = null)
    {
        if (condition)
            throw new AssertionException(message ?? "Expected false but got true");
    }

    public static void Equal<T>(T expected, T actual, string? message = null)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new AssertionException(message ?? $"Expected {expected} but got {actual}");
    }

    public static void NotEqual<T>(T expected, T actual, string? message = null)
    {
        if (EqualityComparer<T>.Default.Equals(expected, actual))
            throw new AssertionException(message ?? $"Expected values to be different but both were {expected}");
    }

    public static void NotNull(object? obj, string? message = null)
    {
        if (obj == null)
            throw new AssertionException(message ?? "Expected non-null value but got null");
    }

    public static void Null(object? obj, string? message = null)
    {
        if (obj != null)
            throw new AssertionException(message ?? $"Expected null but got {obj}");
    }

    public static void NotEmpty(string? value, string? message = null)
    {
        if (string.IsNullOrEmpty(value))
            throw new AssertionException(message ?? "Expected non-empty string but got empty or null");
    }

    public static void Empty(string? value, string? message = null)
    {
        if (!string.IsNullOrEmpty(value))
            throw new AssertionException(message ?? $"Expected empty string but got '{value}'");
    }

    public static void Contains(string expectedSubstring, string actualString, string? message = null)
    {
        if (!actualString.Contains(expectedSubstring))
            throw new AssertionException(message ?? $"Expected string to contain '{expectedSubstring}' but it didn't");
    }

    public static void Same(object expected, object actual, string? message = null)
    {
        if (!ReferenceEquals(expected, actual))
            throw new AssertionException(message ?? "Expected same reference but got different objects");
    }

    public static void NotSame(object expected, object actual, string? message = null)
    {
        if (ReferenceEquals(expected, actual))
            throw new AssertionException(message ?? "Expected different references but got same object");
    }

    public static void All<T>(IEnumerable<T> collection, Action<T> assertion)
    {
        foreach (var item in collection)
        {
            assertion(item);
        }
    }

    public static async Task<T> ThrowsAsync<T>(Func<Task> testCode) where T : Exception
    {
        try
        {
            await testCode();
            throw new AssertionException($"Expected exception of type {typeof(T).Name} but no exception was thrown");
        }
        catch (T ex)
        {
            return ex;
        }
        catch (Exception ex)
        {
            throw new AssertionException($"Expected exception of type {typeof(T).Name} but got {ex.GetType().Name}: {ex.Message}");
        }
    }

    public static Exception? Record_Exception(Action testCode)
    {
        try
        {
            testCode();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}

static class Record
{
    public static Exception? Exception(Action testCode)
    {
        return Assert.Record_Exception(testCode);
    }
}

class AssertionException : Exception
{
    public AssertionException(string message) : base(message) { }
}
