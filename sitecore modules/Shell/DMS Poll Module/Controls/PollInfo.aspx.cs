using System;

using Sitecore.Data;
using Sitecore.Modules.DMSPoll.Domain;
using Sitecore.Web;

namespace Sitecore.Modules.DMSPoll.Controls
{
   public class PollInfo : System.Web.UI.Page
   {
      protected System.Web.UI.HtmlControls.HtmlGenericControl divCurrentPollClosed;
      protected System.Web.UI.HtmlControls.HtmlGenericControl divArchivedPoll;

      private PollItem currentPoll;
      private PollOptionItem[] currentPollOptionItems;

      public PollItem CurrentPoll
      {
         get
         {
            if (currentPoll == null)
            {
               string currentID = WebUtil.GetQueryString("id");
               currentPoll = new PollItem(PollConstants.DatabaseContext.GetItem(new ID(currentID)));
            }
            return currentPoll;
         }
      }

      public PollOptionItem[] CurrentPollOptionItems
      {
         get
         {
            if (currentPollOptionItems == null)
            {
               currentPollOptionItems = CurrentPoll.Options;
            }
            return currentPollOptionItems;
         }
      }

      protected void Page_Load(object sender, EventArgs e)
      {
         if (CurrentPoll.IsClosed)
         {
            divCurrentPollClosed.Visible = true;
         }
         if (CurrentPoll.IsArchived)
         {
            divArchivedPoll.Visible = true;
         }
      }
   }
}