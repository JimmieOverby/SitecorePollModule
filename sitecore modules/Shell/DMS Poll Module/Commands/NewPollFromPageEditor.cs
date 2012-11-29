using Sitecore.Publishing;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.DMSPoll.Commands
{
   public class NewPollFromPageEditor : AddPoll
   {
      protected override bool ShowPageEditorFunctionality
      {
         get
         {
            return true;
         }
      }
      protected override void StartWizard(ClientPipelineArgs args)
      {
         //If User switched from Preview mode he have no authorisation cookie and so
         //he cannot request nesessary for wizard pages. We have to restore him.
         if (PreviewManager.GetShellUser() != string.Empty) 
         {
            PreviewManager.RestoreUser();
         }
         base.StartWizard(args);
         if (args.HasResult)
         {
            SheerResponse.Eval("window.top.location.reload();");
         }
      }
   }
}
