


## Assumptions

The estimate below assumes:

-   500 users in the tenant
    -   user has sends 1 question per week (asks the bot, then escalates to an SME)
-   SME team assigns the ticket, then closes it
-   SME team uses the messaging extension 25 times per week, across the entire team

## [](/wiki/costestimate#sku-recommendations)SKU recommendations

The recommended SKUs for a production environment are:

-   QnAMaker: Standard (S0)
-   App Service: Standard (S1)
-   Azure Search: Basic
    -   Create up to 14 knowledge bases
    -   The Azure Search service cannot be upgraded in place, so select a tier that will meet your anticipated needs.

## [](/wiki/costestimate#estimated-load)Estimated load

**Number of QnA queries**: 500 users * 5 questions/user/month = 2500 questions/month

**Data storage**: 1 GB max

**Table data operations**:

-   Configuration
    -   (2 reads/question * 2500 questions) + (1 read/escalation * 2500 escalations) + (1 read/update * 2 updates/ticket * 2500 tickets) = 10000 reads
    -   (1 write/update * 3 updates/ticket * 2500 tickets) = 7500 writes

## [](/wiki/costestimate#estimated-cost)Estimated cost

**IMPORTANT:**  This is only an estimate, based on the assumptions above. Your actual costs may vary.

Prices were taken from the  [Azure Pricing Overview](https://azure.microsoft.com/en-us/pricing/)  on 15 August 2019, for the West US 2 region.

Use the  [Azure Pricing Calculator](https://azure.com/e/4ce8461223f440cf93db7760868fabcf)  to model different service tiers and usage patterns.


|  Resource |  Tier |  Load |  Monthly price |   
|---|---|---|---|
|  Storage account (Table)| Standard_LRS|< 1GB data, 17,500 operations|  $0.05 + $0.01 = $0.06 |
|Storage account (Blob)|Standard_LRS|< 1GB data, 20,000 operations|$0.03|
|  Bot Channels Registration | F0  |  N/A | Free  |
|  App Service Plan | S1  | 744 hours  | $74.40  |
|||
|  App Service (Messaging Extension)| -|< 1GB data, 17,500  |(charged to App Service Plan)|
|  Application Insights (Messaging Extension) | -  |  < 5GB data | (free up to 5 GB)|
|  App Service (Configuration) | - | | (charged to App Service Plan)    |
|Application Insights (Configuration)|-|< 5GB data|(free up to 5 GB)|
|QnAMaker Cognitive Service|S0||$10|
|Azure Search|B||$75.14|
|App Service (QnAMaker)|F0||(charged to App Service Plan)|
|Application Insights (QnAMaker)||< 5GB data|(free up to 5 GB)|
|**Total**|||**$159.60**|









