using Sitecore.Shell.Framework.Commands;
using Sitecore.Data.Items;
using Sitecore.Web.UI.Sheer;
using System.Collections.Specialized;
using Sitecore.Text;
using Sitecore.Modules.DMSPoll.Domain;
using Sitecore.Pipelines;

namespace Sitecore.Modules.DMSPoll.Commands
{
    public class AddPoll : Command
    {
        protected virtual bool ShowPageEditorFunctionality
        {
            get
            {
                return false;
            }
        }

        #region overridden methods

        private Pipeline _wizardPipeline;

        public override void Execute(CommandContext context)
        {
            //This is fix for Sitecore bugs. It will execute command two times if select it from 
            //dropdown branch menu (Ticket #29)
            if (_wizardPipeline != null)
            {
                _wizardPipeline = null;
                return;
            }
            //This is fix for Sitecore bugs. Method gets empty context.Items collection 
            //if click on branch from dropdown menu (Ticket #29)
            Item item = context.Items.Length == 0 ? Context.Item : context.Items[0];

            NameValueCollection parameters = new NameValueCollection();

            parameters["parentID"] = item.ID.ToString();
            parameters["language"] = item.Language.Name;
            parameters["showpageeditorfunctionality"] = ShowPageEditorFunctionality.ToString();

            _wizardPipeline = Context.ClientPage.Start(this, "StartWizard", parameters);
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (context.Items.Length != 1)
            {
                return CommandState.Disabled;
            }

            Item poll = context.Items[0];
            if (!poll.Access.CanWrite())
            {
                return CommandState.Disabled;
            }
            if (!poll.Access.CanCreate())
            {
                return CommandState.Disabled;
            }
            PollItem pollItem = new PollItem(poll);
            if (pollItem.IsArchived)
            {
                return CommandState.Disabled;
            }

            return CommandState.Enabled;
        }

        #endregion overridden methods

        #region service

        protected virtual void StartWizard(ClientPipelineArgs args)
        {
            if (!SheerResponse.CheckModified())
            {
                return;
            }

            if (!args.IsPostBack)
            {
                UrlString url = new UrlString(UIUtil.GetUri("control:DMSPollWizard"));
                url.Add("id", args.Parameters["parentID"]);
                url.Add("language", args.Parameters["language"]);
                url.Add("showpageeditorfunctionality", args.Parameters["showpageeditorfunctionality"]);

                SheerResponse.ShowModalDialog(url.ToString(), true);
                args.WaitForPostBack();
            }
            else
            {
                _wizardPipeline = null;
                if (args.HasResult)
                {
                    var pollItem = Context.ContentDatabase.GetItem(args.Result);
                    if (pollItem != null)
                    {
                        var itemToRefresh = pollItem.Parent;
                        
                        Context.ClientPage.SendMessage(this, "item:updated(id=" + itemToRefresh.ID + ")");
                        Context.ClientPage.SendMessage(this, "item:refreshchildren(id=" + itemToRefresh.ID + ")");
                    }
                }
            }
        }

        #endregion service
    }
}