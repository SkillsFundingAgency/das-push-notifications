## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Push Notification Function App

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">


[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/_projectname_?branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=_projectid_&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=_projectId_&metric=alert_status)](https://sonarcloud.io/dashboard?id=_projectId_)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/secure/RapidBoard.jspa?rapidView=564&projectKey=_projectKey_)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/_pageurl_)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

The push notification function app is an app that receives and processes push notifications and push request subscriptions inbound from the Outer Api.


## How It Works



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
        "NServiceBusLicense": "<LICENSEKEY",
        "NServiceBusEndpointName": "SFA.DAS.PushNotifications",
        "DbConnectionString": "Data Source=.;Initial Catalog=SFA.DAS.PushNotifications.Database;Integrated Security=True"
    }
} 
```

Azure Table Storage config

Row Key: SFA.DAS.Tools.Servicebus.Support_1.0

Partition Key: LOCAL

Data:

```json
{
  "BaseUrl": "localhost:5001",
  "UserIdentitySettings":{
    "RequiredRole": "Servicebus Admin", 
    "UserSessionExpiryHours": 24,
    "UserRefreshSessionIntervalMinutes": 5,
    "NameClaim": "name"
  },
  "ServiceBusSettings":{
    "ServiceBusConnectionString": "",
    "QueueSelectionRegex": "[-,_]+error",
    "PeekMessageBatchSize": 10,
    "MaxRetrievalSize": 250,
    "ErrorQueueRegex": "[-,_]error[s]*$",
    "RedactPatterns": [
      "(.*SharedAccessKey=)([\\s\\S]+=)(.*)"
    ]
  },
  "CosmosDbSettings":{
    "Url": "",
    "AuthKey": "",
    "DatabaseName": "QueueExplorer",
    "CollectionName": "Session",
    "Throughput": 400,
    "DefaultCosmosOperationTimeout": 55,
    "DefaultCosmosInterimRequestTimeout": 2
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

```

## 🐛 Known Issues

```
*  N/A
```
