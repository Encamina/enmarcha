# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade src\Encamina.Enmarcha.Entities.Abstractions\Encamina.Enmarcha.Entities.Abstractions.csproj
4. Upgrade src\Encamina.Enmarcha.Core\Encamina.Enmarcha.Core.csproj
5. Upgrade src\Encamina.Enmarcha.AI.Abstractions\Encamina.Enmarcha.AI.Abstractions.csproj
6. Upgrade src\Encamina.Enmarcha.Entities\Encamina.Enmarcha.Entities.csproj
7. Upgrade src\Encamina.Enmarcha.DependencyInjection\Encamina.Enmarcha.DependencyInjection.csproj
8. Upgrade src\Encamina.Enmarcha.AI.TextsTranslation.Abstractions\Encamina.Enmarcha.AI.TextsTranslation.Abstractions.csproj
9. Upgrade src\Encamina.Enmarcha.Net.Http\Encamina.Enmarcha.Net.Http.csproj
10. Upgrade src\Encamina.Enmarcha.Conversation\Encamina.Enmarcha.Conversation.csproj
11. Upgrade src\Encamina.Enmarcha.Agents.Abstractions\Encamina.Enmarcha.Agents.Abstractions.csproj
12. Upgrade src\Encamina.Enmarcha.AI\Encamina.Enmarcha.AI.csproj
13. Upgrade src\Encamina.Enmarcha.AI.OpenAI.Azure\Encamina.Enmarcha.AI.OpenAI.Azure.csproj
14. Upgrade src\Encamina.Enmarcha.SemanticKernel\Encamina.Enmarcha.SemanticKernel.csproj
15. Upgrade src\Encamina.Enmarcha.Email.Abstractions\Encamina.Enmarcha.Email.Abstractions.csproj
16. Upgrade src\Encamina.Enmarcha.Agents\Encamina.Enmarcha.Agents.csproj
17. Upgrade src\Encamina.Enmarcha.SemanticKernel.Connectors.Document\Encamina.Enmarcha.SemanticKernel.Connectors.Document.csproj
18. Upgrade src\Encamina.Enmarcha.Testing\Encamina.Enmarcha.Testing.csproj
19. Upgrade src\Encamina.Enmarcha.SemanticKernel.Plugins.Text\Encamina.Enmarcha.SemanticKernel.Plugins.Text.csproj
20. Upgrade src\Encamina.Enmarcha.Data.EntityFramework\Encamina.Enmarcha.Data.EntityFramework.csproj
21. Upgrade src\Encamina.Enmarcha.Data.Cosmos\Encamina.Enmarcha.Data.Cosmos.csproj
22. Upgrade src\Encamina.Enmarcha.SemanticKernel.Plugins.Memory\Encamina.Enmarcha.SemanticKernel.Plugins.Memory.csproj
23. Upgrade src\Encamina.Enmarcha.Testing.Smtp\Encamina.Enmarcha.Testing.Smtp.csproj
24. Upgrade src\Encamina.Enmarcha.Email.MailKit\Encamina.Enmarcha.Email.MailKit.csproj
25. Upgrade src\Encamina.Enmarcha.Services.Abstractions\Encamina.Enmarcha.Services.Abstractions.csproj
26. Upgrade src\Encamina.Enmarcha.Agents.Skills.QuestionAnswering\Encamina.Enmarcha.Agents.Skills.QuestionAnswering.csproj
27. Upgrade samples\SemanticKernel\Encamina.Enmarcha.Samples.SemanticKernel.DocumentContentExtractor\Encamina.Enmarcha.Samples.SemanticKernel.DocumentContentExtractor.csproj
28. Upgrade src\Encamina.Enmarcha.Aspire\Encamina.Enmarcha.Aspire.csproj
29. Upgrade tst\Encamina.Enmarcha.SemanticKernel.Connectors.Document.Tests\Encamina.Enmarcha.SemanticKernel.Connectors.Document.Tests.csproj
30. Upgrade tst\Encamina.Enmarcha.Core.Tests\Encamina.Enmarcha.Core.Tests.csproj
31. Upgrade src\Encamina.Enmarcha.AspNet.OpenApi\Encamina.Enmarcha.AspNet.OpenApi.csproj
32. Upgrade tst\Encamina.Enmarcha.SemanticKernel.Tests\Encamina.Enmarcha.SemanticKernel.Tests.csproj
33. Upgrade tst\Encamina.Enmarcha.AI.Tests\Encamina.Enmarcha.AI.Tests.csproj
34. Upgrade samples\SemanticKernel\Encamina.Enmarcha.Samples.SemanticKernel.Text\Encamina.Enmarcha.Samples.SemanticKernel.Text.csproj
35. Upgrade samples\Data\Encamina.Enmarcha.Samples.Data.EntityFramework\Encamina.Enmarcha.Samples.Data.EntityFramework.csproj
36. Upgrade samples\Data\Encamina.Enmarcha.Samples.Data.CosmosDB\Encamina.Enmarcha.Samples.Data.CosmosDB.csproj
37. Upgrade src\Encamina.Enmarcha.SemanticKernel.Connectors.Memory\Encamina.Enmarcha.SemanticKernel.Connectors.Memory.csproj
38. Upgrade src\Encamina.Enmarcha.SemanticKernel.Plugins.Chat\Encamina.Enmarcha.SemanticKernel.Plugins.Chat.csproj
39. Upgrade src\Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering\Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering.csproj
40. Upgrade src\Encamina.Enmarcha.Data.Qdrant\Encamina.Enmarcha.Data.Qdrant.csproj
41. Upgrade tst\Encamina.Enmarcha.Email.MailKit.Tests\Encamina.Enmarcha.Email.MailKit.Tests.csproj
42. Upgrade tst\Encamina.Enmarcha.Net.Http.Tests\Encamina.Enmarcha.Net.Http.Tests.csproj
43. Upgrade tst\Encamina.Enmarcha.Services.Abstractions.Tests\Encamina.Enmarcha.Services.Abstractions.Tests.csproj
44. Upgrade tst\Encamina.Enmarcha.Entities.Abstractions.Tests\Encamina.Enmarcha.Entities.Abstractions.Tests.csproj
45. Upgrade src\Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle\Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle.csproj
46. Upgrade src\Encamina.Enmarcha.AspNet.Mvc\Encamina.Enmarcha.AspNet.Mvc.csproj
47. Upgrade src\Encamina.Enmarcha.AspNet.Mvc.Formatters\Encamina.Enmarcha.AspNet.Mvc.Formatters.csproj
48. Upgrade src\Encamina.Enmarcha.AI.TextsTranslation.Azure\Encamina.Enmarcha.AI.TextsTranslation.Azure.csproj
49. Upgrade src\Encamina.Enmarcha.AI.QuestionsAnswering.Azure\Encamina.Enmarcha.AI.QuestionsAnswering.Azure.csproj
50. Upgrade src\Encamina.Enmarcha.AI.LanguagesDetection.Azure\Encamina.Enmarcha.AI.LanguagesDetection.Azure.csproj
51. Upgrade src\Encamina.Enmarcha.AI.IntentsPrediction.Azure\Encamina.Enmarcha.AI.IntentsPrediction.Azure.csproj

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                        | Current Version | New Version | Description                                   |
|:----------------------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Aspire.Hosting                                      | 8.2.2           |             | Replace with Aspire.Hosting.AppHost 13.0.2    |
| Aspire.Hosting.AppHost                              |                 | 13.0.2      | Replacement for Aspire.Hosting                |
| Azure.Identity                                      | 1.13.2          | 1.17.1      | Deprecated version, update recommended        |
| Microsoft.AspNetCore.Authentication.JwtBearer       | 8.0.14          | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.AspNetCore.Authentication.OpenIdConnect   | 8.0.14          | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.EntityFrameworkCore                       | 8.0.14          | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.EntityFrameworkCore.SqlServer             | 8.0.14          | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Azure                          | 1.11.0          | 1.13.1      | Deprecated version, update recommended        |
| Microsoft.Extensions.Caching.Abstractions           | 8.0.0           | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Configuration                  | 8.0.0           | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Configuration.Abstractions     | 8.0.0           | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.DependencyInjection.Abstractions | 8.0.2; 9.0.8  | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Hosting                        | 8.0.1           | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Http                           | 8.0.0; 8.0.1    | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Logging.Abstractions           | 8.0.3           | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Options                        | 8.0.2           | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Options.ConfigurationExtensions | 8.0.0          | 10.0.1      | Recommended for .NET 10.0                     |
| Microsoft.Extensions.Options.DataAnnotations        | 8.0.0           | 10.0.1      | Recommended for .NET 10.0                     |
| Newtonsoft.Json                                     | 13.0.3          | 13.0.4      | Recommended for .NET 10.0                     |
| SixLabors.ImageSharp                                | 3.1.7           | 3.1.12      | Security vulnerability                        |
| System.ComponentModel.Annotations                   | 5.0.0           |             | Functionality included with framework         |
| System.Configuration.ConfigurationManager           | 8.0.1           | 10.0.1      | Recommended for .NET 10.0                     |
| System.Memory.Data                                  | 8.0.1           | 10.0.1      | Recommended for .NET 10.0                     |
| System.Net.Http.Json                                | 8.0.1           | 10.0.1      | Recommended for .NET 10.0                     |
| System.Numerics.Tensors                             | 8.0.0           | 10.0.1      | Recommended for .NET 10.0                     |
| System.Text.Json                                    | 8.0.5           | 10.0.1      | Recommended for .NET 10.0                     |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### src\Encamina.Enmarcha.Entities.Abstractions\Encamina.Enmarcha.Entities.Abstractions.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `8.0.2` to `10.0.1` (*recommended for .NET 10.0*)
  - System.ComponentModel.Annotations should be removed (*functionality included with framework*)

#### src\Encamina.Enmarcha.Core\Encamina.Enmarcha.Core.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Configuration.Abstractions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - System.Configuration.ConfigurationManager should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - System.Text.Json should be updated from `8.0.5` to `10.0.1` (*recommended for .NET 10.0*)
  - System.ComponentModel.Annotations should be removed (*functionality included with framework*)

#### src\Encamina.Enmarcha.AI.Abstractions\Encamina.Enmarcha.AI.Abstractions.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Options should be updated from `8.0.2` to `10.0.1` (*recommended for .NET 10.0*)
  - System.ComponentModel.Annotations should be removed (*functionality included with framework*)

#### src\Encamina.Enmarcha.Entities\Encamina.Enmarcha.Entities.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Logging.Abstractions should be updated from `8.0.3` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.DependencyInjection\Encamina.Enmarcha.DependencyInjection.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `8.0.2` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.AI.TextsTranslation.Abstractions\Encamina.Enmarcha.AI.TextsTranslation.Abstractions.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `8.0.2` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.Net.Http\Encamina.Enmarcha.Net.Http.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### src\Encamina.Enmarcha.Conversation\Encamina.Enmarcha.Conversation.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Caching.Abstractions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.Agents.Abstractions\Encamina.Enmarcha.Agents.Abstractions.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### src\Encamina.Enmarcha.AI\Encamina.Enmarcha.AI.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Options should be updated from `8.0.2` to `10.0.1` (*recommended for .NET 10.0*)
  - System.Numerics.Tensors should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.AI.OpenAI.Azure\Encamina.Enmarcha.AI.OpenAI.Azure.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Hosting should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options should be updated from `8.0.2` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.DataAnnotations should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - System.ComponentModel.Annotations should be removed (*functionality included with framework*)
  - Microsoft.Extensions.Azure should be updated from `1.11.0` to `1.13.1` (*deprecated version*)

#### src\Encamina.Enmarcha.SemanticKernel\Encamina.Enmarcha.SemanticKernel.csproj modifications

Project properties changes:
  - Target framework should be changed from `net6.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Hosting should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.ConfigurationExtensions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.DataAnnotations should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.Email.Abstractions\Encamina.Enmarcha.Email.Abstractions.csproj modifications

NuGet packages changes:
  - System.ComponentModel.Annotations should be removed (*functionality included with framework*)

#### src\Encamina.Enmarcha.Agents\Encamina.Enmarcha.Agents.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### src\Encamina.Enmarcha.SemanticKernel.Connectors.Document\Encamina.Enmarcha.SemanticKernel.Connectors.Document.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Http should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.ConfigurationExtensions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.DataAnnotations should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - System.Memory.Data should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - SixLabors.ImageSharp should be updated from `3.1.7` to `3.1.12` (*security vulnerability*)

#### src\Encamina.Enmarcha.Testing\Encamina.Enmarcha.Testing.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Options should be updated from `8.0.2` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.SemanticKernel.Plugins.Text\Encamina.Enmarcha.SemanticKernel.Plugins.Text.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### src\Encamina.Enmarcha.Data.EntityFramework\Encamina.Enmarcha.Data.EntityFramework.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `8.0.14` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.Data.Cosmos\Encamina.Enmarcha.Data.Cosmos.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Configuration.Abstractions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Hosting should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options should be updated from `8.0.2` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.ConfigurationExtensions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.DataAnnotations should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Newtonsoft.Json should be updated from `13.0.3` to `13.0.4` (*recommended for .NET 10.0*)
  - Azure.Identity should be updated from `1.13.2` to `1.17.1` (*deprecated version*)

#### src\Encamina.Enmarcha.SemanticKernel.Plugins.Memory\Encamina.Enmarcha.SemanticKernel.Plugins.Memory.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### src\Encamina.Enmarcha.Testing.Smtp\Encamina.Enmarcha.Testing.Smtp.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Logging.Abstractions should be updated from `8.0.3` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.Email.MailKit\Encamina.Enmarcha.Email.MailKit.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Configuration.Abstractions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Hosting should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Logging.Abstractions should be updated from `8.0.3` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.ConfigurationExtensions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.DataAnnotations should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.Services.Abstractions\Encamina.Enmarcha.Services.Abstractions.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Configuration.Abstractions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `8.0.2` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Logging.Abstractions should be updated from `8.0.3` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.Agents.Skills.QuestionAnswering\Encamina.Enmarcha.Agents.Skills.QuestionAnswering.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### samples\SemanticKernel\Encamina.Enmarcha.Samples.SemanticKernel.DocumentContentExtractor\Encamina.Enmarcha.Samples.SemanticKernel.DocumentContentExtractor.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Hosting should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.Aspire\Encamina.Enmarcha.Aspire.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Aspire.Hosting should be removed and replaced with Aspire.Hosting.AppHost `13.0.2` (*deprecated, out of support*)

#### tst\Encamina.Enmarcha.SemanticKernel.Connectors.Document.Tests\Encamina.Enmarcha.SemanticKernel.Connectors.Document.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### tst\Encamina.Enmarcha.Core.Tests\Encamina.Enmarcha.Core.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### src\Encamina.Enmarcha.AspNet.OpenApi\Encamina.Enmarcha.AspNet.OpenApi.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### tst\Encamina.Enmarcha.SemanticKernel.Tests\Encamina.Enmarcha.SemanticKernel.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### tst\Encamina.Enmarcha.AI.Tests\Encamina.Enmarcha.AI.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### samples\SemanticKernel\Encamina.Enmarcha.Samples.SemanticKernel.Text\Encamina.Enmarcha.Samples.SemanticKernel.Text.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### samples\Data\Encamina.Enmarcha.Samples.Data.EntityFramework\Encamina.Enmarcha.Samples.Data.EntityFramework.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.EntityFrameworkCore should be updated from `8.0.14` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.EntityFrameworkCore.SqlServer should be updated from `8.0.14` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Hosting should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)

#### samples\Data\Encamina.Enmarcha.Samples.Data.CosmosDB\Encamina.Enmarcha.Samples.Data.CosmosDB.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Hosting should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.SemanticKernel.Connectors.Memory\Encamina.Enmarcha.SemanticKernel.Connectors.Memory.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.DependencyInjection.Abstractions should be updated from `9.0.8` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Http should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.SemanticKernel.Plugins.Chat\Encamina.Enmarcha.SemanticKernel.Plugins.Chat.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Options should be updated from `8.0.2` to `10.0.1` (*recommended for .NET 10.0*)
  - Newtonsoft.Json should be updated from `13.0.3` to `13.0.4` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering\Encamina.Enmarcha.SemanticKernel.Plugins.QuestionAnswering.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### src\Encamina.Enmarcha.Data.Qdrant\Encamina.Enmarcha.Data.Qdrant.csproj modifications

Project properties changes:
  - Target framework should be changed from `net6.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Http should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)

#### tst\Encamina.Enmarcha.Email.MailKit.Tests\Encamina.Enmarcha.Email.MailKit.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### tst\Encamina.Enmarcha.Net.Http.Tests\Encamina.Enmarcha.Net.Http.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### tst\Encamina.Enmarcha.Services.Abstractions.Tests\Encamina.Enmarcha.Services.Abstractions.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Configuration should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)

#### tst\Encamina.Enmarcha.Entities.Abstractions.Tests\Encamina.Enmarcha.Entities.Abstractions.Tests.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### src\Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle\Encamina.Enmarcha.AspNet.OpenApi.Swashbuckle.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### src\Encamina.Enmarcha.AspNet.Mvc\Encamina.Enmarcha.AspNet.Mvc.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - Microsoft.AspNetCore.Authentication.JwtBearer should be updated from `8.0.14` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.AspNetCore.Authentication.OpenIdConnect should be updated from `8.0.14` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.AspNet.Mvc.Formatters\Encamina.Enmarcha.AspNet.Mvc.Formatters.csproj modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

#### src\Encamina.Enmarcha.AI.TextsTranslation.Azure\Encamina.Enmarcha.AI.TextsTranslation.Azure.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Configuration.Abstractions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Hosting should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Http should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.ConfigurationExtensions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.DataAnnotations should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - System.Net.Http.Json should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - System.Text.Json should be updated from `8.0.5` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.AI.QuestionsAnswering.Azure\Encamina.Enmarcha.AI.QuestionsAnswering.Azure.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Caching.Abstractions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Configuration.Abstractions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Hosting should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.ConfigurationExtensions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.DataAnnotations should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.AI.LanguagesDetection.Azure\Encamina.Enmarcha.AI.LanguagesDetection.Azure.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Configuration.Abstractions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Hosting should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Http should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.DataAnnotations should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - System.Net.Http.Json should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)

#### src\Encamina.Enmarcha.AI.IntentsPrediction.Azure\Encamina.Enmarcha.AI.IntentsPrediction.Azure.csproj modifications

NuGet packages changes:
  - Microsoft.Extensions.Configuration.Abstractions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Hosting should be updated from `8.0.1` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.ConfigurationExtensions should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
  - Microsoft.Extensions.Options.DataAnnotations should be updated from `8.0.0` to `10.0.1` (*recommended for .NET 10.0*)
