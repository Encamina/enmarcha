# Running the Cosmos DB with ENMARCHA

The Cosmos DB ENMARCHA Nuget provides a highly useful transparent service to query information in Azure Cosmos DB databases. Azure Cosmos DB is a fully managed NoSQL and relational database for modern app development. This guide will assist you in executing the sample.

## Setup

- Use or generate a Cosmos DB in Azure. To generate a new CosmosDB, see [Azure Cosmos DB documentation](https://azure.microsoft.com/es-es/free/cosmos-db/search/).
- Install Visual Studio. For more information, see [Visual Studio](https://visualstudio.microsoft.com).

## Steps

Here are the steps to follow to run the example properly:

- Use or generate a new Cosmos DB database in Azure. Store the name of the database, the endpoint and the authorization key with read-write privileges in `appsettings.json`.
- Create a new Container in the database called `BILLS`, keep the default configuration.
- Open the solution `enmarcha.sln` in the start folder with Visual Studio.
- Go to Samples > Data right-click `Encamina.Enmarcha.Samples.Data.CosmosDB` project and click Set as Startup project.
- Click on the run button.

Once the program is running, a console will be displayed waiting for inputs. You can then indicate the actions you want to do.

## Further information

More documentation can be found in the [Encamina.Enmarcha.Data.Cosmos](
../../../src/Encamina.Enmarcha.Data.Cosmos/README.md) project.