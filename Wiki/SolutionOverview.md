
![Architecture Image](/wiki/images/architecture_overview.png)

The **HR Support** application has the following main components:

-   **QnAMaker**: Resources that comprise the QnAMaker cognitive service, which implements the "FAQ" part of the application. The installer creates a [knowledge base](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/concepts/knowledge-base) using the [tools](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/concepts/development-lifecycle-knowledge-base) provided by QnA Maker.
-   **HR Support Bot**: The bot serves both end users and subject matter experts.

-   The knowledge base (KB) in QnA Maker is presented to end users in a 1:1 conversational bot interface. Through the bot, the user can escalate to a designated expert team, send feedback about the app, or give feedback on specific answers.
-   The expert team receives notifications from the bot when users create questions or feedback items. The bot tracks questions in a simple "ticketing system", with a basic lifecycle of Unassigned -> Assigned to expert -> Closed. The bot notifies both the user and the expert team as the request changes state.
-   The same bot also implements a messaging extension that lets members of the expert team search for tickets.
-   The Carousel card of recommended tile based on matching tags and its ranking is presented to end users in a 1:1 conversational bot interface.

-   **HR Support Tab:** HR Support is a newly added tab in HR Support bot which shows all the recommended links related to HR in the form of Tiles. User can just click on it and the link will open in the browser in a new tab. User have to Authenticate first to access this tab.
-   **Configuration Application**: An Azure App Service that lets app admins to configure the welcome text, Knowledge Base and Team ID for Bot. The Grid which contains the HR Support tiles data, is newly added functionality where admin can view, update, add, delete the HR Support details.

**QnA Maker**

HR Support uses QnA Maker to respond to user questions; in fact, you must have a QnA Maker knowledge base to start using HR Support. The precision and recall of the bot responses to user questions are directly tied to the quality of the knowledge base, so it's important to follow QnA Maker's recommended [best practices](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/concepts/best-practices). Keep in mind that a good knowledge base requires curation and feedback: see [Development lifecycle of a knowledge base](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/concepts/development-lifecycle-knowledge-base).

For more details about QnAMaker, please refer to the [QnAMaker documentation](https://docs.microsoft.com/en-us/azure/cognitive-services/qnamaker/overview/overview).

**Bot and Messaging Extension**

The bot is built using the [Bot Framework SDK v4 for .NET](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0) and [ASP.NET Core 2.](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.0)1. The bot has a conversational interface in personal (1:1) scope for end-users. It also implements a messaging extension with [query commands](https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/messaging-extensions/search-extensions), which the expert team can use to search for and share requests.

**Tab**

The tab is built in [ASP.NET Core 2.](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.0)1 and [Bot Framework SDK v4 for .NET](https://docs.microsoft.com/en-us/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0). The tab UI has a conversational interface in personal (1:1) scope for end-users. The Tab UI is implemented in React Js in which the data is fetched from Azure Table storage. The silent AAD implementation is done through adal.js

**Configuration App**

The configuration app is a standard [ASP.NET MVC 5](https://docs.microsoft.com/en-us/aspnet/mvc/mvc5) web app. The configuration app will be used infrequently, so the included ARM template puts it in the same App Service Plan as the bot and QnAMaker.

From this simple web interface, app administrators can:

-   Designate the expert team.
-   Set the knowledge base to query.
-   Set the welcome message that's sent to all users.
-   Set the content of the HR Support tab.