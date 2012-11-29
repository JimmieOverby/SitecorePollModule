/* *********************************************************************** *
 * File   : PollVotesCache.cs                             Part of Sitecore *
 * Version: 1.1.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: Represents the cache of poll votes                             *
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
using System.Linq;
using System.Web.Caching;
using Sitecore.Analytics;
using Sitecore.Data;
using Sitecore.Web;

namespace Sitecore.Modules.DMSPoll.Domain
{
    public class PollVotesCache
    {
        private const string PageEventsString = "_PageEvents";
        private const string SessionPollsString = "_SessionPolls";

        public IQueryable<DMSPollData.Analytics.PageEvent> PageEvents
        {
            get
            {
                return WebUtil.CurrentPage.Cache[PollConstants.PollVotesCachePrefix + PageEventsString] as IQueryable<DMSPollData.Analytics.PageEvent>;
            }
        }

        public IQueryable<string> SessionPolls
        {
            get
            {
                return WebUtil.CurrentPage.Cache[PollConstants.PollVotesCachePrefix + SessionPollsString] as IQueryable<string>;
            }
        }

        /// <summary>
        /// Gets the cache settings.
        /// </summary>
        /// <param name="pollVotesCacheSettingID">The poll votes cache setting ID.</param>
        /// <returns></returns>
        public int GetCacheSettings (ID pollVotesCacheSettingID)
        {
            var settingsItem = PollConstants.DatabaseContext.GetItem(PollConstants.PollVotesCacheSettings);
            if (settingsItem != null)
            {
                return MainUtil.GetInt(settingsItem[pollVotesCacheSettingID], 0);
            }
            return 0;
        }


        /// <summary>
        /// Caches the page events.
        /// </summary>
        /// <param name="pageEvents">The page events.</param>
        public void CachePageEvents(IQueryable<DMSPollData.Analytics.PageEvent> pageEvents)
        {
            var votesCount = pageEvents.Count();

            if (votesCount > 100)
            {
                int cacheTime = (votesCount <= 1000) ? GetCacheSettings(PollConstants.PollVotesCache100SettingField) : GetCacheSettings(PollConstants.PollVotesCache1000MoreSettingField);
                if (cacheTime > 0)
                    WebUtil.CurrentPage.Cache.Add(PollConstants.PollVotesCachePrefix + PageEventsString, pageEvents, null,
                                              Cache.NoAbsoluteExpiration, new TimeSpan(0, cacheTime, 0),
                                              CacheItemPriority.Low, null);
            }
        }

        /// <summary>
        /// Caches the session polls.
        /// </summary>
        /// <param name="sessionPolls">The session polls.</param>
        public void CacheSessionPolls(IQueryable<string> sessionPolls)
        {
            var votesCount = sessionPolls.Count();
            if (votesCount > 100)
            {
                int cacheTime = (votesCount <= 1000) ? GetCacheSettings(PollConstants.PollVotesCache100SettingField) : GetCacheSettings(PollConstants.PollVotesCache1000MoreSettingField);
                if (cacheTime > 0)
                    WebUtil.CurrentPage.Cache.Add(PollConstants.PollVotesCachePrefix + SessionPollsString, sessionPolls, null,
                                                  Cache.NoAbsoluteExpiration, new TimeSpan(0, cacheTime, 0), CacheItemPriority.Low, null);
            }
        }

    }
}
