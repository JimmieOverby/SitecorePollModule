/* *********************************************************************** *
 * File   : PollAnalyticsDataProvider.cs                  Part of Sitecore *
 * Version: 1.1.0                                         www.sitecore.net *
 *                                                                         *
 *                                                                         *
 * Purpose: Represents the provider for work with analytics data           *
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

using System.Linq;
using Sitecore.Analytics;
using Sitecore.Analytics.Data;
using Sitecore.Diagnostics;
using Sitecore.Web;

namespace Sitecore.Modules.DMSPoll.Domain
{
    public class PollAnalyticsDataProvider
    {

        /// <summary>
        /// Gets the votes count.
        /// </summary>
        /// <param name="option">The option item.</param>
        /// <returns></returns>
        public virtual int GetVotesCount(PollOptionItem option)
        {
            IQueryable<DMSPollData.Analytics.PageEvent> pageEvents = GetVotesList(option.Poll);
            Assert.IsNotNull(pageEvents, "PageEvents");
            int voteCount = pageEvents.Where(pageEvent => pageEvent.DataCode == option.Index).Count();
            return voteCount + MergeVisitEvents(pageEvents, option);
            //return 0;
        }

        /// <summary>
        /// Gets the votes count.
        /// </summary>
        /// <param name="poll">The poll.</param>
        /// <returns></returns>
        public virtual int GetVotesCount(PollItem poll)
        {
            IQueryable<DMSPollData.Analytics.PageEvent> pageEvents = GetVotesList(poll);
            Assert.IsNotNull(pageEvents, "PageEvents");
            return pageEvents.Count() + MergeVisitEvents(pageEvents, poll);
            
            //return 0;
        }

        /// <summary>
        /// Gets the votes list.
        /// </summary>
        /// <param name="poll">The poll.</param>
        /// <returns></returns>
        private static IQueryable<DMSPollData.Analytics.PageEvent> GetVotesList(PollItem poll)
        {
            var pollCache = new PollVotesCache();

            IQueryable<DMSPollData.Analytics.PageEvent> pageEvents = pollCache.PageEvents;
            if (pageEvents == null)
            {
                var analyticsContext = new DMSPollData.Analytics.Data.AnalyticsDataContext();
                pageEvents =
                    analyticsContext.PageEvents.Where(
                        pageEvent =>
                        (pageEvent.PageEventDefinitionId == PollConstants.PollPageEventDefinitionID.ToGuid()) &&
                        (pageEvent.DataKey == poll.Name));
                pollCache.CachePageEvents(pageEvents);
            }

            return pageEvents;
        }

        private static int MergeVisitEvents(IQueryable<DMSPollData.Analytics.PageEvent> pageevents, PollItem poll)
        {
          var currentvisitevents =
            Tracker.DataContext.PageEvents.Where(
              x => x.PageEventDefinitionId == PollConstants.PollPageEventDefinitionID.ToGuid() &&
              x.DataKey == poll.Name);
          var count = 0;
          foreach (var currentvisitevent in currentvisitevents)
          {

            var c = pageevents.Where(x => x.PageEventId == currentvisitevent.PageEventId).Count();
            count += c;
          }
          return currentvisitevents.Count() - count;
        }

        private static int MergeVisitEvents(IQueryable<DMSPollData.Analytics.PageEvent> pageevents, PollOptionItem option)
        {
            var currentvisitevents =
              Tracker.DataContext.PageEvents.Where(
                x => x.PageEventDefinitionId == PollConstants.PollPageEventDefinitionID.ToGuid() &&
                x.DataCode == option.Index);
            var count = 0;
            foreach (var currentvisitevent in currentvisitevents)
            {

                var c = pageevents.Where(x => x.PageEventId == currentvisitevent.PageEventId).Count();
                count += c;
            }
            return currentvisitevents.Count() - count;
        }

        /// <summary>
        /// Check if poll with the name is exists.
        /// </summary>
        /// <param name="pollName">Name of the poll.</param>
        /// <returns></returns>
        public virtual bool PollExists(string pollName)
        {
            var analyticsContext = new DMSPollData.Analytics.Data.AnalyticsDataContext();
            var pageEvents =
                analyticsContext.PageEvents.Where(
                    pageEvent =>
                    (pageEvent.PageEventDefinitionId == PollConstants.PollPageEventDefinitionID.ToGuid()) &&
                    (pageEvent.DataKey == pollName));
            return pageEvents.Count() > 0;
        }

        /// <summary>
        /// Check if votes is exists.
        /// </summary>
        /// <param name="poll">The poll.</param>       
        /// <returns></returns>
        public virtual bool VoteExists(PollItem poll)
        {
            IQueryable<string> sessionPolls;
            IQueryable<string> trackerPols;
            var pollCache = new PollVotesCache();
            trackerPols = pollCache.SessionPolls;
            sessionPolls = pollCache.SessionPolls;
            if (sessionPolls == null)
            {
                trackerPols = (from pe in Tracker.DataContext.PageEvents
                                  where pe.DataKey == poll.Name
                                  select new string(pe.DataKey.ToCharArray())).AsQueryable<string>();
                if (trackerPols.Count() == 0)
                {
                    var analyticsGlobalCookie = WebUtil.GetCookieValue("SC_ANALYTICS_GLOBAL_COOKIE");
                    var analyticsContext = new DMSPollData.Analytics.Data.AnalyticsDataContext();
                    sessionPolls =
                        (from pageEvent in analyticsContext.PageEvents
                         join page in analyticsContext.Pages on pageEvent.PageId equals page.PageId
                         join session in analyticsContext.Visits on page.VisitId equals session.VisitId
                         join globalSession in analyticsContext.Visitors on session.VisitorId equals
                             globalSession.VisitorId
                         where pageEvent.PageEventDefinitionId == PollConstants.PollPageEventDefinitionID.ToGuid()
                               && globalSession.VisitorId.ToString().Replace("-", "").Replace("{", "").Replace("}", "").ToLower() == analyticsGlobalCookie
                         select new string(pageEvent.DataKey.ToCharArray())).Distinct();

                    pollCache.CacheSessionPolls(sessionPolls);
                }
                else
                {
                    pollCache.CacheSessionPolls(trackerPols);
                }

                
            }
            var exists = false;

            if (sessionPolls != null)
            {
                var list = sessionPolls.ToList();
                exists = list.Contains(poll.Name);
            }else if (trackerPols != null)
            {
                var list = trackerPols.ToList();
                exists = list.Contains(poll.Name);
            }
            
            return exists;
        }
    }
}
