
# General template issues

**Generic possible issues**

There are certain issues that can arise that are common to many of the app templates. Please check [here](https://github.com/OfficeDev/microsoft-teams-stickers-app/wiki/Troubleshooting) for reference to these.

**Problems deploying to Azure**

**1. Error when attempting to reuse a Microsoft Azure AD application ID for the bot registration**

Bot is not valid. Errors: MsaAppId is already in use.

-   Creating the resource of type Microsoft.BotService/botServices failed with status "BadRequest"

This happens when the Microsoft Azure application ID entered during the setup of the deployment has already been used and registered for a bot.

**Fix**

Either register a new Microsoft Azure AD application or delete the bot registration that is currently using the attempted Microsoft Azure application ID.

**Problems  in configurator app**

**1. Not able to see images in drop-down list while adding grid data**

This happens when the images are not uploaded properly while deployment.

**Fix**

Please verify whether the images are existing in blob storage.
- Go to [ azure portal](http://portal.azure.com/)
- Go to Storage Accounts
- Open storage explorer & expand Blob Containers
- Check for the images, if doesn't exists, please click on upload button and   upload the images.

![Troubleshooting](/wiki/images/DropDown_Image_Troubleshooting.png)


**Didn't find your problem here?**
Please, report the issue [here](https://github.com/OfficeDev/microsoft-teams-faqplusplus-app/issues/new)


**2. Facing issues in Configurator app**

If facing any issue related to application.

**Fix**

Please go to app-insights and check for errors.
- Go to [ azure portal](http://portal.azure.com/)
- Go to App-insights related to your app.
- Open Logs (Analytics)
-  Select Time Range & fire the query from different tables like exceptions, customEvent etc. like below:

![Troubleshooting](/wiki/images/TelemetryAnalytics.png)


**Didn't find your problem here?**
Please, report the issue [here](https://github.com/OfficeDev/microsoft-teams-faqplusplus-app/issues/new)



**Problems  in bot/tab app**

If facing any issue related to bot/tab.

**Fix**

Please go to app-insights and check for errors.
- Go to [ azure portal](http://portal.azure.com/)
- Go to App-insights related to your app.
- Open Logs (Analytics)
-  Select Time Range & fire the query from different tables like exceptions, customEvent etc. like below:

![Troubleshooting](/wiki/images/TelemetryAnalytics.png)


**Didn't find your problem here?**
Please, report the issue [here](https://github.com/OfficeDev/microsoft-teams-faqplusplus-app/issues/new)