﻿// <copyright file="WelcomeTeamCard.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Cards
{
    using System.Collections.Generic;
    using System.Globalization;
    using AdaptiveCards;
    using Microsoft.Bot.Schema;
    using Microsoft.Teams.Apps.AskHR.Common;
    using Microsoft.Teams.Apps.AskHR.Models;
    using Microsoft.Teams.Apps.AskHR.Properties;

    /// <summary>
    ///  This class process  Welcome Card when installed in Team scope.
    /// </summary>
    public static class WelcomeTeamCard
    {
        /// <summary>
        /// This method will construct the welcome team card when bot is added to the team.
        /// </summary>
        /// <returns>Team welcome card.</returns>
        public static Attachment GetCard()
        {
            var textAlignment = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? AdaptiveHorizontalAlignment.Right : AdaptiveHorizontalAlignment.Left;

            AdaptiveCard teamWelcomeCard = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = Resource.WelcomeTeamCardContent,
                        Wrap = true,
                        HorizontalAlignment = textAlignment
                    }
                },
                Actions = new List<AdaptiveAction>
                {
                    // Team- take a tour submit action.
                    new AdaptiveSubmitAction
                    {
                        Title = Resource.TakeATeamTourButtonText,
                        Data = new TeamsAdaptiveSubmitActionData
                        {
                            MsTeams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                DisplayText = Resource.TakeATeamTourButtonText,
                                Text = Constants.TeamTour
                            }
                        },
                    }
                }
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = teamWelcomeCard,
            };
        }
    }
}