using System;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Web;

namespace Sitecore.Modules.DMSPoll
{
    /// <summary>
    /// Constants related to the Poll module
    /// </summary>
    public class PollConstants
    {
        public static Database DatabaseContext
        {
            get
            {
                return Context.ContentDatabase ?? Context.Database;
            }
        }
       
        public const string PollCssPath = "/sitecore modules/shell/DMS Poll Module/Poll.css";

        public const string ContentPath = "/sitecore/content";

        //poll templates constants
        public static readonly ID PollTemplateID = ID.Parse("{459759A3-54D1-4037-ABBF-4E8D98C9E7D9}");
        public static readonly ID PollOptionTemplateID = ID.Parse("{69C26EB6-C818-4D85-9570-BAC54479BDE8}");
        public static readonly ID PollArchiveTemplateID = ID.Parse("{DD41F44E-497F-412D-8290-7D1FD85BD5FA}");

        //poll item constants
        public static readonly ID PollItemClosedField = ID.Parse("{052EEC6A-15C5-4BAD-8062-36EDFD9DDA44}");
        public static readonly ID PollItemOneVoteField = ID.Parse("{3FBD0A7A-AE78-4586-86FC-4FD9CB5AA991}");        
        public static readonly ID PollItemIntroField = ID.Parse("{114A7498-E0BD-4F06-8EF4-4B7BD4CE7D75}");
        public static readonly ID PollItemThankYouField = ID.Parse("{F776F02E-E4A2-495D-BF34-9CD5035B71FC}");
        public static readonly ID PollItemCookiesRequiredField = ID.Parse("{A226A782-9F63-490A-9B6B-779D6740BAF3}");
        public static readonly ID PollItemCookiesRequiredTextField = ID.Parse("{CC1126BE-37CF-40EE-B017-AA8B9DCA3B8C}");
        
        public const string PollItemCookiesRequiredText ="Required cookies text";

        //poll option constants
        public static readonly ID PollOptionItemTitleField = ID.Parse("{BE5B9DDA-871C-48F9-8604-8CB62D3CBCBC}");
        public static readonly ID PollOptionItemIndexField = ID.Parse("{2926CE34-A03E-4DF4-AA1B-EF97D4F5346E}");        
        
        //'Poll' page event definition
        public static readonly ID PollPageEventDefinitionID = ID.Parse("{87061f63-8603-4d37-abd9-54cb2ab90b5c}");

        //poll renderings constants
        public static readonly string PollRenderingID = "{B81AEFBD-EE51-45A3-AD83-ED4EED6D510C}";
        public static readonly string PollRenderingDefaultDevice = "{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}";
        public const string PollPathParameter = "PollPath";
        public const string PollArchiveFolderName = "Arhive";

        //poll cache constants
        public const string PollVotesCachePrefix = "PollVotesCache";
        public static readonly ID PollVotesCacheSettings = ID.Parse("{DDE87207-7DDF-427C-AF3B-A4064D69753E}");
        public static readonly ID PollVotesCache100SettingField = ID.Parse("{0861D134-2E3C-4AC2-AE19-528DD8113949}");
        public static readonly ID PollVotesCache1000MoreSettingField = ID.Parse("{25AEC078-858D-409B-9D8D-571316DC1DF5}");        
    }
}