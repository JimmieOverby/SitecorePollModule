/* *********************************************************************** *
 * File   : PollItem.cs                                   Part of Sitecore *
 * Version: 1.1.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: Represents a poll item in the Content Editor                   *
 *                                                                         *
 * Copyright (C) 1999-2009 by Sitecore A/S. All rights reserved.           *
 *                                                                         *
 * This work is the property of:                                           *
 *                                                                         *
 *        Sitecore A/S                                                     *
 *        Meldahlsgade 5, 4.                                               *
 *        1613 Copenhagen V.                                               *
 *        Denmark                                                          *
 *                                                                         *
 * This is a Sitecore published work under Sitecore's                      *
 * shared source license.                                                  *
 *                                                                         *
 * *********************************************************************** */
using System;
using System.Collections;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.DMSPoll.Domain
{
    /// <summary>
    /// A poll item
    /// </summary>
    public class PollItem : CustomItemBase
    {
        #region Data section

        private readonly PollAnalyticsDataProvider DataProvider = new PollAnalyticsDataProvider();

        private PollOptionItem[] pollOptionItems;

        public bool IsOneVoteForUser
        {
            get
            {
                return MainUtil.GetBool(InnerItem[PollConstants.PollItemOneVoteField], false);
            }
        }


        public bool IsClosed
        {
            get
            {
                return MainUtil.GetBool(InnerItem[PollConstants.PollItemClosedField], false);

            }
            set
            {
                using (new EditContext(InnerItem))
                {
                    InnerItem[PollConstants.PollItemClosedField] = value ? "1" : "0";
                }
            }
        }

        public bool IsCookiesRequired
        {
            get
            {
                return MainUtil.GetBool(InnerItem[PollConstants.PollItemCookiesRequiredField], false);

            }
            set
            {
                using (new EditContext(InnerItem))
                {
                    InnerItem[PollConstants.PollItemCookiesRequiredField] = value ? "1" : "0";
                }
            }
        }

        public string CookiesRequiredText
        {
            get
            {
                return InnerItem[PollConstants.PollItemCookiesRequiredTextField];
            }
            set
            {
                using (new EditContext(InnerItem))
                {
                    InnerItem[PollConstants.PollItemCookiesRequiredTextField] = value;
                }
            }
        }

        public bool IsArchived
        {
            get { return InnerItem.Parent.TemplateID == PollConstants.PollArchiveTemplateID; }
        }

        public string Intro
        {
            get
            {
                return InnerItem[PollConstants.PollItemIntroField];
            }
            set
            {
                using (new EditContext(InnerItem))
                {
                    InnerItem[PollConstants.PollItemIntroField] = value;
                }
            }
        }

        public string ThankYou
        {
            get
            {
                return InnerItem[PollConstants.PollItemThankYouField];
            }
            set
            {
                using (new EditContext(InnerItem))
                {
                    InnerItem[PollConstants.PollItemThankYouField] = value;
                }
            }
        }

        public new string DisplayName
        {
            get
            {
                return (this as CustomItemBase).DisplayName;
            }
            set
            {
                using (new EditContext(InnerItem))
                {
                    InnerItem["__Display name"] = value;
                }
            }
        }

        #endregion Data section

        #region vote persistence

        private int? voteCount;
        public int VoteCount
        {
            get
            {
                if (!voteCount.HasValue)
                {
                    voteCount = DataProvider.GetVotesCount(this);
                }

                return voteCount.Value;
            }
        }

        #endregion vote persistence

        public PollOptionItem[] Options
        {
            get
            {
                if (pollOptionItems != null)
                {
                    return pollOptionItems;
                }

                ArrayList list = new ArrayList();

                foreach (Item item in InnerItem.Children)
                {
                    if ((item.TemplateID == PollConstants.PollOptionTemplateID) && (item.Versions.Count != 0))
                    {
                        list.Add((new PollOptionItem(item, this)));
                    }
                }
                pollOptionItems = (PollOptionItem[])list.ToArray(typeof(PollOptionItem));
                return pollOptionItems;
            }
        }

        public PollOptionItem this[int index]
        {
            get
            {
                foreach (var pollOptionItem in Options)
                {
                    if (pollOptionItem.Index == index)
                    {
                        return pollOptionItem;
                    }
                }
                return null;
            }
        }

        public PollItem(Item item)
            : base(item)
        {
        }
        
        /// <summary>
        /// Check if votes exists.
        /// </summary>
        /// <returns></returns>
        public bool VoteExists()
        {
            return DataProvider.VoteExists(this);
        }

        /// <summary>
        /// Archives current poll to the pollItem.
        /// </summary>
        /// <param name="pollItem">The poll item.</param>
        public void ArchiveTo(Item pollItem)
        {
            //copy archive folder if exists
            foreach (Item item in InnerItem.Children)
            {
                if (item.TemplateID == PollConstants.PollArchiveTemplateID)
                {
                    item.CopyTo(pollItem, item.Name);
                    item.Delete();
                }
            }
            //or create archive folder 
            if (pollItem.Children[PollConstants.PollArchiveFolderName] == null)
            {
                TemplateItem pollArhiveTemplate = PollConstants.DatabaseContext.Templates[PollConstants.PollArchiveTemplateID];
                Assert.IsNotNull(pollArhiveTemplate, "Poll archive template");

                pollItem.Add(PollConstants.PollArchiveFolderName, pollArhiveTemplate);
            }
            string itemName = string.Format("{0} {1}", InnerItem.Name, DateTime.Today.ToShortDateString().Replace('.', ' '));
            string displayName = itemName;

            if (ItemUtil.GetItemNameError(itemName).Length > 0)
                itemName = ItemUtil.ProposeValidItemName(itemName);

            Item currentPollCopy = InnerItem.CopyTo(pollItem.Children[PollConstants.PollArchiveFolderName], itemName, Data.ID.NewID, false);

            if (displayName != itemName)
            {
                PollItem archivedPoll = new PollItem(currentPollCopy) { DisplayName = displayName };
            }

            //copy all poll options
            foreach (Item item in InnerItem.Children)
            {
                item.CopyTo(currentPollCopy, item.Name);
                item.Delete();
            }
            InnerItem.Delete();
        }

        #region helpers

        public static implicit operator PollItem(Item item)
        {
            if (item != null)
            {
                return new PollItem(item);
            }
            return null;
        }

        #endregion helpers        
    }
}