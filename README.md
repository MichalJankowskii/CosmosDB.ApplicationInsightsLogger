# CosmosDB.ApplicationInsightsLogger
[![Build Status](https://travis-ci.org/MichalJankowskii/CosmosDB.ApplicationInsightsLogger.svg?branch=master)](https://travis-ci.org/MichalJankowskii/CosmosDB.ApplicationInsightsLogger.Helpers)
[![Downloads](https://img.shields.io/nuget/dt/CosmosDB.LoggingExtension.svg)](https://github.com/MichalJankowskii/CosmosDB.ApplicationInsightsLogger)

The library that provides methods that will help you with monitoring and logging CosmosDB with ApplicationInsights. With you will be able to gather information about the cost of CosmosDb queries and plan optimisations on the database.
## Installation - NuGet Packages
```
Install-Package CosmosDB.ApplicationInsightsLogger
```

## Usage
Usage is straightforward:
```csharp
CosmosDbExtension.DependencyName = "your dependency name";

IQueryable<Family> query = client.CreateDocumentQuery<Family>(
                    UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName),
                    "SELECT * FROM Family WHERE Family.lastName = 'Veum'",
                    queryOptions);

query.ExecuteWithStatsLogging("name of query").Result

```
Please remember that you can assign _DependencyName_ and also _command name_ connected to each query. Especially the second option is attractive because you can find and measure queries from the same category.

And in the ApplicationInsights you can use the following query for getting data:
```
dependencies 
| where type == "CosmosDB"
| extend commandName = customDimensions["commandName"]
| extend requestCharge = todouble(customDimensions["requestCharge"])
| order by timestamp desc 
| project timestamp, target, type, name, data, commandName, requestCharge, duration
```
Results will look like this:
![alt text](https://github.com/MichalJankowskii/CosmosDB.ApplicationInsightsLogger/blob/master/media/CosmosDbStatsResult.png)


