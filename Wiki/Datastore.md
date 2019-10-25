
# Data stores

The app uses the following data stores:  
1. Azure Storage Account  
- [Table] Storage for bot related configurations (welcome message, KbId, TeamId, and grid data that contains bot tab tiles related information)  
- [Table] For tracking all of the requests and the necessary actions that impact a request - i.e. request is assigned to a SME.  
2. Blob storage contains images that we are showing in drop-down list in configurator web app while configuring the tile.

All these resources are created in your Azure subscription. None are hosted directly by Microsoft.

# [](/wiki/Datastore#storage-account)Storage account

## [](/wiki/Datastore#configurationinfo-table)HelpInfo Table

The **HelpInfo** table stores data about the necessary configurations that are required for the bot. The table has the following rows.

| Attribute | Comment |  
|--|--|  
|PartitionKey |This represents the partition key of the azure storage table|  
|RowKey| Represents the unique id of each row|  
|Timestamp| Contains the Date time of row creation|  
|Description| Contains the description of the tile |  
|ImageUrl| Contains the URL of the image shown in the tile |  
|IsEdit| Boolean flag that represents whether a existing tile is edited |  
|RedirectUrl| Contains the url to which the user will redirect once he clicks on the tiles shown in the tab |  
|Tags| Keywords that we used to filter the tile according to the question, if the question doesn't exist in QnA Maker Service |  
|TileOrder|Represents the order of the tile in tab and configurator as per they are created through the configurator (admin) app |  
|Title| The title of the tile shown in the tab |  


  
## [](/wiki/Datastore#ConfigurationInfo-table)ConfigurationInfo Table
| Attribute | Comment |  
|--|--| 
|KnowledgeBaseId| This is the knowledge base Id for which the bot can return answers from the QnA Maker. |  
| MSTeamId| The team Id which the bot can be able to post messages whenever the end user asks for an expert's assistance with a query.| 
| WelcomeMessage | The welcome message is a configurable text that the bot would send to the user the very first time that a user installs the bot in a personal scope.|
## [](/wiki/Datastore#ticketinfo-table)Tickets Table

The **Tickets** table stores data about tickets (or requests) that are posted to the SME Team by the bot on behalf of a user. Each row in the table has the following columns:  
| Attribute | Comment |  
|--|--|  
|PartitionKey |This represents the partition key of the azure storage table|  
|RowKey| Represents the unique id of each row|  
|Timestamp| Contains the Date time of row creation|  
| TicketId | The ticket ID. |  
| Status | An integer value. |  
| Title | The end user title provided. |  
| DateCreated | The date when a new ticket is created. |  
| Description | The description text that is written by the end user. |  
| RequesterName | The name of the end user when a new ticket is created. |  
| RequesterUserPrincipalName | The email address of the end user.|  
| RequesterGivenName | The first name of the end user |  
| RequesterConversationId | The conversationId of the 1:1 chat between the end user and the HR Support bot. |  
| SmeCardActivityId | The activityId when the new ticket adaptive card is posted in the General channel of the SME team. |  
| SmeThreadConversationId | The conversationId in the SME team General channel at the time a new ticket is created. |    
| LastModifiedByName | The name of the SME user who recently updated the ticket. |  
| LastModifiedByObjectId | The AAD Object ID of the SME user who recently updated the ticket. |  
| UserQuestion | The original question that has been asked by the end user. |  
| KnowledgeBaseAnswer| The answer that is stored in the knowledge base. |
