namespace DMSPollData.Analytics.Data
{
    using System.Configuration;

  public class AnalyticsDataContext : DMSPollData.Analytics.AnalyticsDataContext
    {
        public AnalyticsDataContext():base(ConnectionString)
        {
            Connection.ConnectionString = ConnectionString;
        }

        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["analytics"].ConnectionString;
            }
        }
    }
}
