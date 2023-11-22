# Running the EntityFramework ENMARCHA Nuget sample

The EntityFramework ENMARCHA Nuget provides a similar Microsoft Entity Framework experience using SQL Server databasde. SQL Server is a relational database management system (RDBMS) developed by Microsoft. This guide will assist you in executing the sample.


## Prerequisites

- Use or create a Azure SQL Server database. To create a new Azure SQL Server database, see [Azure SQL Server Documentation](https://azure.microsoft.com/es-es/products/azure-sql/database/).
- Install Visual Studio. For more information, see [Visual Studio](https://visualstudio.microsoft.com).

## Steps

Here are the steps to follow to run the example properly:

- Use or generate a new SQL server database in Azure. Use the tool of your choices to create the tables. Use this SQL command to create them:

```sql
CREATE TABLE Employee
(
    Id nvarchar(20) PRIMARY KEY,
    Fullname nvarchar(50) NOT NULL
);

CREATE TABLE BILL
(
    Id nvarchar(20) PRIMARY KEY,
    Concept nvarchar(20) NOT NULL
    Amount float NOT NULL
    EmployeeId nvarchar(20),
    FOREIGN KEY (EmployeeId) REFERENCES Employee(Id)
);
```
- Go to the database general information, copy the ConnectionString and store it in `appsettings.json`.
- Open the solution `enmarcha.sln` in the start folder with Visual Studio.
- Go to Samples > Data right-click `Sample_Data_CosmosDB` project and click Set as Startup project.
- Click on the run button.

Once the program is running, a console will be displayed waiting for inputs. You can then indicate the actions you want to do.

## Further information

More documentation can be found in the [Encamina.Enmarcha.Data.EntityFramework](
../../../src/Encamina.Enmarcha.Data.EntityFramework/README.md) project.