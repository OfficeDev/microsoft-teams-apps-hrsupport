

**HR Support**

Chatbots are an easy way to provide answers to frequently asked questions by users. However, most chatbots fail to engage with users in meaningful way because there is no human in the loop when the chatbot fails. HR Support bot is a friendly Q&A bot that brings a human in the loop when it is unable to help.

HR Support is divided in to three parts.

 - Bot

- HR Support Tab

- Configurator Web App

**Bot**

One can ask the bot a question and the bot responds with an answer if it is contained in the knowledge base. If not it then matches the message with tags from configurator app. The tiles with matched tags are shown as a carousel of adaptive cards based on ranking. If not, the bot allows the user to submit a query which then gets posted in a pre-configured team of experts who are help to provide support by acting upon the notifications from within their team itself.

**Applicable scenarios:** HR Support works really well for light weight QnA and helpdesk scenarios. It also works well to provide quick support when launching new projects/initiatives in the organization.

An end-user interacting with HR Support:

![HR Support (experts view)](/wiki/images/Bot.png)

**HR Support Tab**

HR Support is a tab added in HR Support bot which shows all the recommended links related to HR in the form of Tiles. User can just click on it and the link will open in the browser in a new tab. Whatever data the Admin has added in Configurator will be shown in the tiles of HR Support. A HR Support Tile will contain a title, image and Description of a recommendation which will be fetched from configurator web application.

The UI for HR Support is shown below:

![HR Support tab](/wiki/images/Tab.png)

**Configurator Web App:**

The configurator Application is an Admin application in which Admin can Add, Update Delete and View HR Support Data. Admin can set the welcome text message for Bot which the end user will see when he/she will open the Bot. Admin can set the knowledge base ID where Bot will search the queries entered by User. Admin can also set the Team ID.

Configurator Web App UI :

![Configurator Web App](/wiki/images/configurator.png)

-   [Solution overview](/wiki/SolutionOverview)

      -   [Data stores](/wiki/Datastore)
      -   [Cost estimate](/wiki/costestimates)

-   Deploying the app

    -   [Deployment guide](/wiki/DeployementGuide)
    -  [Troubleshooting](/wiki/troubleshooting)