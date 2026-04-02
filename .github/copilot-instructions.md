# Project Guidelines — ENMARCHA

ENMARCHA is a modular .NET NuGet package library that accelerates .NET development with enterprise-grade utilities for AI, Semantic Kernel, data, and cloud scenarios. The AI layer is branded **ENGENIA**.

## Architecture

**Abstractions + Implementation separation**: Every feature area follows a strict split:
- `Encamina.Enmarcha.{Feature}.Abstractions` — interfaces, base records/classes, options. Targets `netstandard2.1` for broad compatibility.
- `Encamina.Enmarcha.{Feature}.{Provider}` — concrete implementations (e.g., `AI.OpenAI.Azure`, `Data.Cosmos`, `Email.MailKit`). Targets `net10.0`.

**Namespace conventions**:
- Source: `Encamina.Enmarcha.{FeatureArea}{.SubArea}` matching the project name.
- DI extension methods must live in `namespace Microsoft.Extensions.DependencyInjection;` for discoverability.
- Tests: `Encamina.Enmarcha.{FeatureArea}.Tests`.

**Key directories**:
- `src/` — All library projects.
- `tst/` — All test projects.
- `samples/` — Example apps demonstrating module usage.

## Build and Test

- **.NET SDK 10.0** (`global.json` — `rollForward: latestFeature`).
- Build: `dotnet build Enmarcha.sln`
- Test: `dotnet test Enmarcha.sln`
- Local NuGet publish: `.\PublishToLocal.ps1`
- Current version set in `Directory.Build.props` `VersionPrefix`.

## Code Style

StyleCop Analyzers and `.editorconfig` enforce style at build time. Key rules:

- **Always use `var`** (all three var preferences are `:warning`).
- **File-scoped namespaces** (`namespace X;` not `namespace X { }`).
- **4-space indentation**, UTF-8 with BOM, CRLF line endings.
- **Expression-bodied members** encouraged for trivial implementations.
- **Modern C# patterns**: pattern matching, switch expressions, null-check over type-check.
- **XML documentation is mandatory** on all public APIs (`GenerateDocumentationFile` is enabled; `CS1591` is only suppressed in test projects).
- **Nullable reference types enabled** globally — annotate all nullability.
- **Implicit usings enabled** — don't add `using System;` etc.
- **`sealed`** on final implementation classes and test classes.
- **`init`** properties on configuration/options types for immutability.

## Conventions

### Dependency Injection

Expose services via `IServiceCollectionExtensions.cs` in an `Extensions/` folder:

```csharp
namespace Microsoft.Extensions.DependencyInjection;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddFeatureName(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<FeatureOptions>()
                .Bind(configuration.GetSection(nameof(FeatureOptions)))
                .ValidateDataAnnotations()
                .ValidateOnStart();

        services.TryAddSingleton<IFeature, FeatureImpl>();
        return services;
    }
}
```

Provide overloads accepting both `IConfiguration` and `Action<TOptions>`. Return `IServiceCollection` for chaining.

### Options / Configuration

- Use **records** or classes with `init` properties and **Data Annotations** for validation (`[Required]`, `[Url]`, custom validators like `[NotEmptyOrWhitespace]`, `[RequiredIf]`).
- Suffix: `*Options` (e.g., `SemanticTextSplitterOptions`).
- Base types in Abstractions, sealed concrete types in implementations.

### Interfaces

- `I` prefix (`ITextSplitter`, `ICognitiveService`).
- Keep interfaces focused and minimal; compose via inheritance (`ICognitiveService : INameable`).

### Project internal structure

```
src/Encamina.Enmarcha.{Feature}/
├── Extensions/
│   ├── IServiceCollectionExtensions.cs
│   └── Resources/        (resx for exception/validation messages)
├── Internals/             (implementation details)
├── Options/               (configuration classes)
├── {PublicTypes}.cs
└── README.md
```

### Testing

- **xUnit** with parallel execution enabled.
- Test project: `Encamina.Enmarcha.{Feature}.Tests` in `tst/`.
- Test class: `{ClassUnderTest}Tests` (sealed).
- Test method: `{Method}_{Scenario}_{Expected}` (e.g., `Calculate_Percentile_Succeeds`).
- Use `[Theory]`/`[InlineData]` for parameterized tests, `[Fact]` for single-case.
- Collection fixtures via `ICollectionFixture<T>` for shared state.
- Tests suppress `CS1591` and `SA1600` — XML docs not required in tests.

### Versioning

Semantic Versioning. Breaking changes go in major versions with explicit `### Breaking Changes` in `CHANGELOG.md`. See [CONTRIBUTING.md](../CONTRIBUTING.md) for the full contribution workflow.
