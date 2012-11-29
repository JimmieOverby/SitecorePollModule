using System;
using System.Text;
using System.Web.UI;
using System.Web;
using System.Linq;
using System.Collections.Specialized;
using Sitecore.Globalization;
using Sitecore.Modules.DMSPoll.Domain;
using Sitecore.Data.Items;
using Sitecore.Modules.DMSPoll.Exceptions;
using Sitecore.Web;
using Sitecore.Diagnostics;

namespace Sitecore.Modules.DMSPoll.Controls
{
  using System.Web.UI.HtmlControls;

  public class PollVotingSublayout : UserControl, ICallbackEventHandler
    {
        protected System.Web.UI.HtmlControls.HtmlGenericControl divContent;
        protected System.Web.UI.HtmlControls.HtmlGenericControl divResults;
        protected System.Web.UI.HtmlControls.HtmlGenericControl divVoting;
        protected System.Web.UI.HtmlControls.HtmlGenericControl divThankYou;
        protected System.Web.UI.HtmlControls.HtmlGenericControl divButtons;

        private PollOptionItem[] pollOptionItem;
        private Exception ajaxException;
        
        private string PathToPoll
        {
            get
            {
                string attributes = Attributes["sc_parameters"];

                NameValueCollection renderingParameters = WebUtil.ParseUrlParameters(attributes);

                if (string.IsNullOrEmpty(attributes) || renderingParameters[PollConstants.PollPathParameter] == null)
                {
                    var pollSublayout = Parent as Web.UI.WebControls.Sublayout;

                    if (pollSublayout != null)
                        attributes = pollSublayout.DataSource;
                    return attributes;
                }

                return renderingParameters[PollConstants.PollPathParameter];
            }
        }

        public PollOptionItem[] PollOptionItems
        {
            get
            {
                if (pollOptionItem == null)
                {
                    pollOptionItem = CurrentPoll.Options;
                }
                return pollOptionItem;
            }
        }

        public PollItem CurrentPoll { get; set; }

        public bool IsPollOptionsExists
        {
            get { return PollOptionItems.Count() != 0; }
        }

        public bool IsPollExistsInCurrentLanguage
        {
            get
            {
                Item pollItem = CurrentPoll.InnerItem;
                return pollItem.Versions.Count != 0;
            }
        }

        public string RenderControl(Control control)
        {
            try
            {
                bool ctrlVisible = control.Visible;
                control.Visible = true;

                string result = HtmlUtil.RenderControl(control);
                control.Visible = ctrlVisible;
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("PollVotingSublayout.ascx Render exception", ex, this);
                Visible = false;
                return string.Empty;
            }
        }

        public string GetCallbackResult()
        {
            string htmlResult = string.Empty;
            try
            {
                htmlResult = RenderControl(divResults);
            }
            catch (Exception e)
            {
                Log.Error("Poll module GetCallbackResult exception", e, this);
                ajaxException = e;
            }
            finally
            {
                if (ajaxException != null)
                {
                    htmlResult = String.Format("<b style='color:red;'>Poll module error: {0}</b>", ajaxException.Message);
                }
            }
            return htmlResult;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            try
            {
                Vote(new Guid(eventArgument));
            }
            catch (Exception e)
            {
                Log.Error("Poll module RaiseCallbackEvent exception", e, this);
                ajaxException = e;
            }
        }

        public void Page_Load(object sender, EventArgs e)
        {
            try
            {
                CurrentPoll = GetCurrentPoll();

                if (Page.IsCallback)
                {
                    return;
                }

                if (CurrentPoll.IsClosed)
                {
                    Visible = false;
                    return;
                }

                RegisterCssScript();

                if (CurrentPoll.IsCookiesRequired)
                {
                    Page.ClientScript.RegisterStartupScript(GetType(), "DMSCheckCookies", "DMSPoll.CheckCookie();", true);
                }

                if (CurrentPoll.IsOneVoteForUser && CurrentPoll.VoteExists())
                {
                    divButtons.Visible = false;
                    divThankYou.Visible = false;
                    divContent.InnerHtml = RenderControl(divResults);
                }
                else {
                    divContent.InnerHtml = RenderControl(divVoting);                    
                }

                string cbReference = Page.ClientScript.GetCallbackEventReference(this, "arg", "DMSPoll.ReceiveServerData", "context");
                string callbackScript = string.Format("if(DMSPoll == undefined) {{var DMSPoll = new Object();}} DMSPoll.CallServer = function (arg, context) {{ {0}; }}; ", cbReference);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "DMSCallServer", callbackScript, true);
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "DMSJavascriptVariables", GetJavascriptVariables(), true);

                
            }
            catch (MissedItemException)
            {
                Visible = false;
            }
            catch (Exception ex)
            {
                Log.Error("PollVotingSublayout.ascx OnLoad Exception", ex, this);
                Visible = false;
            }
        }
        
        private PollItem GetCurrentPoll()
        {
            try
            {
                PollItem poll = new PollItem(PollConstants.DatabaseContext.GetItem(PathToPoll));
                return poll;
            }
            catch (ArgumentNullException e)
            {
                throw new MissedItemException(String.Format("Invalid {0} parameter", PollConstants.PollPathParameter), e);
            }
        }

        private void Vote(Guid optionId)
        {
            for (int i = 0; i < PollOptionItems.Length; i++)
            {
                if (PollOptionItems[i].ID.Guid == optionId)
                {
                    PollOptionItems[i].PlaceVote();
                    return;
                }
            }
            throw new ArgumentOutOfRangeException();
        }


        public void RegisterCssScript()
        {
            var css = new HtmlLink { Href = PollConstants.PollCssPath };
            css.Attributes["rel"] = "stylesheet";
            this.Page.Header.Controls.Add(css);
        }

        private string GetJavascriptVariables()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("DMSPoll.{0} = \"{1}\";", divContent.ID, divContent.ClientID));
            sb.AppendLine(string.Format("DMSPoll.placeVote = \"{0}_PlaceVote\";", ClientID));
            sb.AppendLine(string.Format("DMSPoll.{0} = \"{1}\";", divButtons.ID, divButtons.ClientID));
            sb.AppendLine(string.Format("DMSPoll.clientID = \"{0}\";", ClientID));
            sb.AppendLine(string.Format("DMSPoll.alertText = \"{0}\";", Translate.Text("Vote select option")));
            return sb.ToString();
        }
    }
}
