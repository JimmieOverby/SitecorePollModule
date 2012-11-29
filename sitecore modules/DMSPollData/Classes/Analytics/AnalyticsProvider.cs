using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMSPollData.Analytics
{
    using System.Data.SqlClient;

    public class AnalyticsProvider
    {
        public delegate void ReadDelegate(SqlDataReader reader);
    }
}
