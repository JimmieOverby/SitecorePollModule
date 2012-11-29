/* *********************************************************************** *
 * File   : PollOptionItem.cs                             Part of Sitecore *
 * Version: 1.1.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: The instance of the poll option item                           *
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
using Sitecore.Data.Items;
using Sitecore.Modules.DMSPoll.Exceptions;
using System.Web;

namespace Sitecore.Modules.DMSPoll.Domain
{
    /// <summary>
    /// A poll option
    /// </summary>
    public class PollOptionItem : CustomItemBase
    {
        #region Fields

        private PollItem poll;
        private int? voteCount;
        
        private readonly PollAnalyticsDataProvider dataProvider = new PollAnalyticsDataProvider();

        #endregion Fields

        #region constructor

        public PollOptionItem(Item item) : this(item, null) { }

        public PollOptionItem(Item item, PollItem pItem)
            : base(item)
        {
            poll = pItem;
        }

        #endregion constructor

        #region Data section

        public string Title
        {
            get
            {
                return InnerItem[PollConstants.PollOptionItemTitleField];
            }
            set
            {
                using (new EditContext(InnerItem))
                {                    
                    InnerItem[PollConstants.PollOptionItemTitleField] = value;
                }
            }
        }

        public int Index
        {
            get
            {
                return MainUtil.GetInt(InnerItem[PollConstants.PollOptionItemIndexField], 0);
            }
            set
            {
                using (new EditContext(InnerItem))
                {
                    InnerItem[PollConstants.PollOptionItemIndexField] = value.ToString();
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

        public int Sortorder
        {
            get
            {
                return MainUtil.GetInt(InnerItem["__Sortorder"], 0);
            }
            set
            {
                using (new EditContext(InnerItem))
                {
                    InnerItem["__Sortorder"] = value.ToString();
                }
            }
        }



        #endregion Data section

        #region Vote persistance

        public PollItem Poll
        {
            get
            {
                if (poll == null)
                {
                    poll = InnerItem.Parent;
                }
                return poll;
            }
        }

        #endregion Vote persistance

        public double GetVoteCountPercent()
        {
            double percent = 0;
            int optionVotes = VoteCount;
            if (optionVotes != 0)
            {
                percent = (double)optionVotes / Poll.VoteCount * 100;
            }
            return percent;
        }

        public int VoteCount
        {
            get
            {
                if (!voteCount.HasValue)
                {
                    voteCount = dataProvider.GetVotesCount(this);
                }

                return voteCount.Value;
            }
        }

        public void PlaceVote()
        {
            if (Poll.IsOneVoteForUser && dataProvider.VoteExists(Poll))
                return;

            PollAnalytics.PlaceVote(this);
        }



        #region helpers

        public static implicit operator PollOptionItem(Item item)
        {
            if (item != null)
            {
                return new PollOptionItem(item);
            }
            return null;
        }

        #endregion helpers
    }
}