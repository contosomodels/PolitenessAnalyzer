# GitHub Actions CI/CD Setup

This repository uses GitHub Actions for automated building, testing, and publishing.

## Workflows

### Build, Test, and Publish (`build-and-publish.yml`)

This workflow runs on:
- **Push to `main`**: Runs tests, increments version, and publishes to NuGet
- **Pull Requests to `main`**: Runs tests only (no publishing)

## Setup Required

### 1. Add NuGet API Key as a Secret

To publish packages to NuGet.org, you need to add your NuGet API key as a repository secret:

1. Go to your repository on GitHub
2. Navigate to **Settings** ? **Secrets and variables** ? **Actions**
3. Click **New repository secret**
4. Name: `NUGET_API_KEY`
5. Value: Your NuGet API key from https://www.nuget.org/account/apikeys
6. Click **Add secret**

### 2. Enable Workflow Permissions

The workflow needs write permissions to commit version bumps and create releases:

1. Go to **Settings** ? **Actions** ? **General**
2. Under **Workflow permissions**, select **Read and write permissions**
3. Check **Allow GitHub Actions to create and approve pull requests**
4. Click **Save**

## Workflow Behavior

### On Pull Request
1. ? Restores dependencies
2. ? Builds the solution in Release configuration
3. ? Runs console tests
4. ? Reports test results
5. ? Does NOT publish package

### On Push to Main
1. ? Restores dependencies
2. ? Builds the solution in Release configuration
3. ? Runs console tests
4. ? Generates version number from run number (e.g., `0.1.42-beta`)
5. ? Builds and packs the NuGet package with generated version
6. ? Publishes to NuGet.org
7. ? Creates a GitHub release with the version tag

## Version Management

The workflow automatically generates version numbers based on the **GitHub run number**:
- Format: `0.1.RUN_NUMBER-beta`
- Example: `0.1.42-beta`
  - `0.1` = Fixed major.minor version
  - `42` = GitHub Actions run number (auto-increments)
  - `-beta` = Pre-release suffix

**Benefits:**
- ? No version bump commits needed
- ? Each build gets a unique version
- ? Simple, predictable versioning scheme
- ? Run number ensures uniqueness and monotonic increment

**To change the suffix:**
Edit the workflow file and modify the version generation line:
```powershell
$newVersion = "0.1.$runNumber-beta"  # Change -beta to -alpha, -rc, or remove for stable
```

**To change major.minor version:**
Simply update the prefix in the workflow:
```powershell
$newVersion = "0.2.$runNumber-beta"  # Bump to 0.2.x
$newVersion = "1.0.$runNumber"       # Bump to 1.0.x (stable)
```

## Test Requirements

The console test project must:
- Return exit code `0` for success
- Return exit code `1` (or non-zero) for failure
- The workflow will fail if tests fail, preventing package publication

## Skipping CI

The workflow runs on every push to main. To prevent accidental triggers:
- Be mindful of what you push to main
- Use pull requests for changes that need review
- The workflow will only publish if tests pass

## Troubleshooting

### Tests Fail
- Check the workflow logs in the **Actions** tab
- Run tests locally: `dotnet run --project Contoso.AI.PolitenessAnalyzer.ConsoleTest`

### Package Not Publishing
- Verify the `NUGET_API_KEY` secret is set correctly
- Check that the API key has not expired
- Ensure the API key has push permissions
- Note: Date-based versions should be unique unless you're rebuilding on the same day with the same run number (unlikely)

### Version Bump Not Committed
- No longer applicable - versions are generated automatically without commits
