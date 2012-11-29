using System;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentManager;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Modules.DMSPoll.Domain;
using Sitecore.Data.Items;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.DMSPoll.Commands
{
   // This command is necessary to work around a but in FieldEditor
   // When it does not refresh properly.
   public class RefreshTracking : Command
   {
      public override void Execute(CommandContext context)
      {
         if (Context.Request.FilePath == "/sitecore/shell/~/xaml/sitecore.shell.applications.analytics.trackingfielddetails.aspx")
         {
            SheerResponse.Eval("window.location.href = window.location.href");
         }
      }

      public override CommandState QueryState(CommandContext context)
      {
         return CommandState.Enabled;
      }
   }
}
