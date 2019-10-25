// <copyright file="SuggestedLinkCard.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Microsoft.Teams.Apps.AskHR.Cards
{
    using System;
    using System.Collections.Generic;
    using AdaptiveCards;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Microsoft.Teams.Apps.AskHR.Bots;
    using Microsoft.Teams.Apps.AskHR.Common.Models;
    using Microsoft.Teams.Apps.AskHR.Models;
    using Microsoft.Teams.Apps.AskHR.Properties;

    /// <summary>
    ///  This class handles unrecognized input sent by the user-asking random question to bot and suggest tab links.
    /// </summary>
    public class SuggestedLinkCard
    {
        /// <summary>
        /// This method will construct the card when unrecognized input is sent by the user and suggests tab links.
        /// </summary>
        /// <param name="userQuestion">Actual question asked by the user to the bot.</param>
        /// <param name="tile">Tile returned on matching tags.</param>
        /// <param name="cardMessage">Message to be shown on card.</param>
        /// <returns>Suggested Tab Card.</returns>
        public static Attachment GetCard(string userQuestion, HelpInfoEntity tile, string cardMessage)
        {
            Uri imageUrl = new Uri(tile.ImageUrl != null ? tile.ImageUrl : " ", UriKind.RelativeOrAbsolute);
            Uri redirectUrl = new Uri(tile.RedirectUrl != null ? tile.RedirectUrl : " ", UriKind.RelativeOrAbsolute);

            AdaptiveCard suggestedInputCard = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = cardMessage,
                        Wrap = true
                    },
                    new AdaptiveContainer
                    {
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveTextBlock
                            {
                                Text = tile.Title,
                                Wrap = true,
                                Weight = AdaptiveTextWeight.Bolder
                            },
                            new AdaptiveImage
                            {
                                Url = imageUrl,
                                SelectAction = new AdaptiveOpenUrlAction
                                {
                                    Url = redirectUrl,
                                },
                                AltText = tile.Title
                            },
                            new AdaptiveTextBlock
                            {
                                Text = tile.Description,
                                Wrap = true,
                                MaxLines = 3
                            }
                        }
                    }
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveOpenUrlAction
                    {
                        Title = Resource.ViewHelpArticleButtonText,
                        Url = redirectUrl
                    },
                    new AdaptiveSubmitAction
                    {
                        Title = Resource.AskAnExpertButtonText,
                        Data = new ResponseCardPayload
                        {
                            MsTeams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                DisplayText = Resource.AskAnExpertDisplayText,
                                Text = AskHRBot.AskAnExpert
                            },
                            UserQuestion = userQuestion
                        },
                    },
                    new AdaptiveSubmitAction
                    {
                        Title = Resource.ShareFeedbackButtonText,
                        Data = new ResponseCardPayload
                        {
                            MsTeams = new CardAction
                            {
                                Type = ActionTypes.MessageBack,
                                DisplayText = Resource.ShareFeedbackDisplayText,
                                Text = AskHRBot.ShareFeedback,
                            }
                        }
                    }
                }
            };

            return new Attachment
            {
                ContentType = AdaptiveCard.ContentType,
                Content = suggestedInputCard,
            };
        }

        /// <summary>
        /// Create the set of cards that comprise the tag tour carousel.
        /// </summary>
        /// <param name="userQuestion">Actual question asked by the user to the bot.</param>
        /// <param name="tileList">Tile returned on matching tags.</param>
        /// <param name="cardMessage">Message to be shown on card.</param>
        /// /// <returns>Carousel tile card.</returns>
        public static IMessageActivity GetTagsCarouselCards(string userQuestion, IEnumerable<HelpInfoEntity> tileList, string cardMessage)
        {
            var attachments = new List<Attachment>();
            var reply = MessageFactory.Attachment(attachments);
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            foreach (var tile in tileList)
            {
                Attachment card = CreateTagCard(userQuestion, tile, cardMessage);
                reply.Attachments.Add(card);
            }

            return reply;
        }

        // Create Individual tile card.
        private static Attachment CreateTagCard(string userQuestion, HelpInfoEntity tile, string cardMessage)
        {
            Attachment card = GetCard(userQuestion, tile, cardMessage);
            return card;
        }
    }
}
