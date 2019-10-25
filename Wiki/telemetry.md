# Telemetry

The Teams Bot web app, Bot Tab & Configurator app logs telemetry to  [Azure Application Insights](https://azure.microsoft.com/en-us/services/monitor/). You can go to the respective Application Insights blade of the Azure App Services to view basic telemetry about your services, such as requests, failures, and dependency errors, custom events, traces etc. .

The Teams Bot integrates with Application Insights to gather bot activity analytics, as described  [here](https://blog.botframework.com/2019/03/21/bot-analytics-behind-the-scenes/).

The Teams Bot logs a few kinds of events:

The  `Activity`  event:

-   Basic activity info:  `ActivityId`,  `ActivityType`,  `Event Name`
-   Basic user info:  `From ID`

The  `UserActivity`  event:

-   Basic activity info:  `ActivityId`,  `ActivityType`,  `Event Name`
-   Basic user info:  `UserAadObjectId`
-   Context of how it was invoked:  `ConversationType`,  `TeamId`

The  `ProcessedPairups`  event:

-   Basic activity info:  `PairsNotifiedCount`,  `UsersNotifiedCount`,  `InstalledTeamsCount`,  `Event Name`

From this information you can calculate key metrics:

-   Which teams (team IDs) have the HR support app?
-   How many users are being paired up with the HR Support app?

The **Bot Tab** with Application Insights to gather event activity analytics, as described  [here]((https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)).

The Bot Tab  logs a few kinds of events:

The  `Exceptions`  event:
- Fetch tiles error logging if any exception.

The  `customEvents`  event:
- Tile click event.

The **Configurator** app with Application Insights to gather event activity analytics, as described  [here]((https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)).

The Configurator App logs a few kinds of events:

The  `Exceptions`  event:
- Global exceptions logging.

The  `customEvents`  event:
- CRUD operations logging.


