using Sitecore.Analytics;
using Sitecore.Analytics.Data;
//using Sitecore.Analytics.Extensions.AnalyticsPageExtensions;
using Sitecore.Data.Items;


namespace Sitecore.Modules.DMSPoll.Domain
{
  public class PollAnalytics
  {
    /// <summary>
    /// Places the vote into Analytics.
    /// </summary>
    /// <param name="optionItem">The option item.</param>
    public static void PlaceVote(PollOptionItem optionItem)
    {
      //    //cancel pipeline handler (for CurrentPage)
      //Tracker.CurrentPage.Cancel();

      //    //Submit analytics tracking for poll item
      SubmitPollTracking(optionItem.Poll.InnerItem, optionItem.InnerItem);


      //    //event trigger on new poll option selected
      var peData = new PageEventData("Poll");

      string text = string.Format("Select {0} ({2}) in poll {1}", optionItem.Title, optionItem.Poll.Name, optionItem.Index);
      string data = string.Concat(new object[] { optionItem.Poll.Name, "|", optionItem.Title, "|", optionItem.Index });
      peData.Data = data;
      peData.Text = text;
      peData.DataKey = optionItem.Poll.Name;
      var row = Tracker.CurrentPage.Register(peData);
      row.DataCode = optionItem.Index;

      //    //submit immediatelly
      SubmitAnalyticsImmediatelly();
    }

    /// <summary>
    /// Submits the analytics immediatelly.
    /// </summary>
    private static void SubmitAnalyticsImmediatelly()
    {
      Tracker.Submit();
    }


    /// <summary>
    /// Submits the poll tracking.
    /// </summary>
    /// <param name="pollItem">The poll item.</param>
    /// <param name="optionItem">The option item.</param>
    private static void SubmitPollTracking(Item pollItem, Item optionItem)
    {
      TrackingFieldProcessor processor = new TrackingFieldProcessor();
      processor.Process(pollItem);
      processor.Process(optionItem);
    }
  }
}
