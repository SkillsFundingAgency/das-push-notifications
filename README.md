
# Push Notification Function App

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">




The Push Notification Function App is a utility that receives and processes push notifications and subscriptions requests inbound from the Apprentice App Outer Api.


## How It Works
Subscription requests are sent from the Apprentice App Outer API and placed in a queue via NServiceBus. The Push Notification Function App processes these items within the queue. The Function App connects directly with the sfa.das.push.notifications.database to get and update data. 

## 🚀 Installation

### Pre-Requisites

```
* A clone of this repository
* A code editor that supports Azure functions
* SQL Server Management Studio


```
### Config

```
This utility uses the standard Apprenticeship Service configuration.

```
AppSettings.Development.json file
```json
{
    "SFA.DAS.PushNotifications.Functions": {
        "NServiceBusLicense": "<LICENSEKEY>",
        "NServiceBusEndpointName": "SFA.DAS.PushNotifications",
        "DbConnectionString": "Data Source=.;Initial Catalog=SFA.DAS.PushNotifications.Database;Integrated Security=True"
    }
} 
```

Azure Table Storage config

Row Key: SFA.DAS.PushNotifications.Functions_1.0

Partition Key: LOCAL

Data:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "EnvironmentName": "LOCAL",
    "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true"
  },
  "SFA.DAS.PushNotifications.Functions": {
    "NServiceBusConnectionString": "<INSERTSTRING>",
    "NServiceBusLicense": "",
    "NServiceBusEndpointName": "SFA.DAS.PushNotifications.Functions",
    "DbConnectionString": "<INSERTSTRING>"
  }
    }
```

## 🔗 External Dependencies


```

```

## Technologies
```
* Azure Functions V3
* Azure Table Storage
* Moq
* NUnit
* FluentAssertions
* NServiceBus
```

## 🐛 Known Issues

```
*  N/A
```
