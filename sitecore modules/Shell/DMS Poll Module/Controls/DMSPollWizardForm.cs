using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Modules.DMSPoll.Commands;
using Sitecore.Modules.DMSPoll.Domain;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using Sitecore.Globalization;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Layouts;
using Sitecore.Data;
using Sitecore.Diagnostics;
using System.Web.UI.WebControls;
using Edit = Sitecore.Web.UI.HtmlControls.Edit;
using Literal = Sitecore.Web.UI.HtmlControls.Literal;

namespace Sitecore.Modules.DMSPoll.Controls
{
    public class DMSPollWizardForm : Web.UI.Pages.WizardForm
    {
        #region wizard controls

        protected Scrollbox Parameters;
        protected Edit PollingName;
        protected Literal PollNameSettingsLiteral;
        protected Frame __PLACEHOLDER;
        protected Border PlaceholderHidden;
        protected Listview OptionList;

        #endregion wizard controls

        #region wizard properties

        private string LastId
        {
            get
            {
                return StringUtil.GetString(Parameters.ServerProperties["LastID"]);
            }
            set
            {
                Parameters.ServerProperties["LastID"] = value;
            }
        }

        private string LastFilledId
        {
            get
            {
                return StringUtil.GetString(Parameters.ServerProperties["LastFilledId"]);
            }
            set
            {
                Parameters.ServerProperties["LastFilledId"] = value;
            }
        }

        private string CurrentOption
        {
            get
            {
                return StringUtil.GetString(Parameters.ServerProperties["CurrentOption"]);
            }
            set
            {
                Parameters.ServerProperties["CurrentOption"] = value;
            }
        }

        private SortedList Results
        {
            get
            {
                if (Parameters.ServerProperties["PollOptionResult"] == null)
                {
                    Parameters.ServerProperties["PollOptionResult"] = new SortedList();
                }
                return (SortedList)Parameters.ServerProperties["PollOptionResult"];
            }
        }

        private Item parentItem;
        private Item ParentItem
        {
            get
            {
                if (parentItem == null)
                {
                    parentItem = PollConstants.DatabaseContext.GetItem(new ID(Context.ClientPage.ServerProperties["id"].ToString()), Language.Parse(ItemLanguage));
                }
                return parentItem;
            }
            set
            {
                Context.ClientPage.ServerProperties["id"] = value.ID.ToString();
            }
        }

        private PollItem currentPollItem;
        protected PollItem CurrentPoll
        {
            get
            {
                if (currentPollItem == null)
                {
                    string pollName = GetItemName(PollingName.Value.Trim());
                    currentPollItem = ParentItem.Axes.GetChild(pollName);
                }
                return currentPollItem;
            }
            set { currentPollItem = value; }
        }

        private bool ShowPageEditorFunctionality
        {
            get
            {
                return MainUtil.GetBool(Context.ClientPage.ServerProperties["showpageeditorfunctionality"], false);
            }
            set
            {
                Context.ClientPage.ServerProperties["showpageeditorfunctionality"] = value;
            }
        }

        private string ItemLanguage
        {
            get
            {
                return StringUtil.GetString(Context.ClientPage.ServerProperties["language"]);
            }
            set
            {
                Context.ClientPage.ServerProperties["language"] = value;
            }
        }

        protected bool DMSPollIsExists
        {
            get { return new PollEventHandler().CheckIsPollItemNameDuplicate(PollingName.Value.Trim()); }
        }

        #endregion services

        #region services

        /// <summary>
        /// Creates the poll.
        /// </summary>
        /// <returns></returns>
        private PollItem CreatePoll()
        {
            TemplateItem pollTemplate = PollConstants.DatabaseContext.Templates[PollConstants.PollTemplateID];
            string pollName = GetItemName(PollingName.Value.Trim()).Trim();
            Item item;
            if (ParentItem.TemplateID == PollConstants.PollTemplateID)
            {
                item = ParentItem.Parent.Add(pollName, pollTemplate);
                Assert.IsNotNull(item, "Can't create poll item");
                var archivedPoll = new PollItem(ParentItem);
                archivedPoll.ArchiveTo(item);
            }
            else
                item = ParentItem.Add(pollName, pollTemplate);

            PollItem pollItem = new PollItem(item);
            if (pollName != PollingName.Value.Trim())
                pollItem.DisplayName = HttpContext.Current.Server.HtmlEncode(PollingName.Value.Trim());

            pollItem.Intro = string.Format("<h1>{0}</h1>", PollingName.Value.Trim());

            CreateOptions(pollItem);
            return pollItem;
        }

        /// <summary>
        /// Creates the options.
        /// </summary>
        /// <param name="poll">The poll.</param>
        private void CreateOptions(PollItem poll)
        {
            int i = 0;
            foreach (string nameOption in Results.Values)
            {
                if (!string.IsNullOrEmpty(nameOption))
                {
                    string nameOptionClear = HttpContext.Current.Server.HtmlEncode(nameOption.Trim());

                    string itemName = GetItemName(nameOptionClear);
                    TemplateItem optionTemplate = PollConstants.DatabaseContext.Templates[PollConstants.PollOptionTemplateID];
                    PollOptionItem option = poll.InnerItem.Add(itemName, optionTemplate);
                    option.Title = nameOptionClear;
                    option.Sortorder = i++;
                    option.Index = i;
                    if (nameOptionClear != itemName)
                    {
                        option.DisplayName = nameOptionClear;
                    }
                    SheerResponse.SetDialogValue(poll.ID.ToString());
                }
            }
        }

        /// <summary>
        /// Fills the list view control with an array of options.
        /// </summary>
        /// <param name="options">The options.</param>
        protected void FillOptionList(PollOptionItem[] options)
        {
            if (options == null) return;

            foreach (PollOptionItem option in options)
            {
                // Create and add the new ListviewItem control to the Listview. We have to assign an unique
                // ID to each control on the page
                ListviewItem listItem = new ListviewItem();

                OptionList.Controls.Add(listItem);
                listItem.ID = Control.GetUniqueID("I");

                // Populate the list item with data.
                listItem.Header = option.Name;
                listItem.Icon = option.InnerItem.Appearance.Icon;
                listItem.ColumnValues["title"] = option.Title;
                listItem.ColumnValues["index"] = option.Index;
                listItem.ColumnValues["id"] = option.ID;
            }
            SheerResponse.SetOuterHtml("OptionList", OptionList);
        }

        #endregion

        #region override methods

        /// <summary>
        /// Raises the load event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> instance containing the event data.</param>        
        protected override void OnLoad(EventArgs e)
        {
            if (Context.ClientPage.IsEvent)
            {
                base.OnLoad(e);
                return;
            }
            ItemLanguage = WebUtil.GetQueryString("language");
            ShowPageEditorFunctionality = MainUtil.GetBool(WebUtil.GetQueryString("showpageeditorfunctionality"), false);

            ParentItem = PollConstants.DatabaseContext.Items[WebUtil.GetQueryString("id"), Language.Parse(ItemLanguage)];
            Assert.IsNotNull(ParentItem, "ParentItem");

            if (ShowPageEditorFunctionality)
            {
                __PLACEHOLDER.SourceUri = PlaceholderManager.GetPlaceholdersUrl(ParentItem, string.Empty);
                PlaceholderHidden.Controls.Add(new Web.UI.HtmlControls.Literal("<input ID=\"__LISTVALUE\" Type=\"hidden\" />"));
                PlaceholderHidden.Controls.Add(new Web.UI.HtmlControls.Literal("<input ID=\"__LISTACTIVE\" Type=\"hidden\" />"));
            }

            base.OnLoad(e);
        }


        /// <summary>
        /// Called when the active page changing.
        /// </summary>
        /// <param name="pageFrom">The page from.</param>
        /// <param name="pageTo">The page to.</param>
        /// <returns></returns>
        protected override bool ActivePageChanging(string pageFrom, ref string pageTo)
        {
            if ((pageFrom == "SelectPollName") && (pageTo == "PollOptions") && DMSPollIsExists)
            {
                SheerResponse.Alert(Translate.Text("Poll name duplicate"));
                return false;
            }

            if ((pageFrom == "PollOptions") && (pageTo == "PollOptionsSettings") && (Results.Count == 0))
            {
                SheerResponse.Alert(Translate.Text("Enter dms poll options validator"));
                return false;
            }
            if ((pageFrom == "PollOptionsSettings") && (pageTo == "PollPlaceholderSelect") && !ShowPageEditorFunctionality)
            {
                pageTo = "LastPage";
            }

            return base.ActivePageChanging(pageTo, ref pageFrom);
        }

        /// <summary>
        /// Called when the active page changed.
        /// </summary>
        /// <param name="pageTo">The page to.</param>
        /// <param name="pageFrom">The page from.</param>
        protected override void ActivePageChanged(string pageTo, string pageFrom)
        {
            if ((pageFrom == "SelectPollName") && (pageTo == "PollOptions") && (LastId == string.Empty))
            {
                LastFilledId = AddPollOptionEditControl();
                LastId = AddPollOptionEditControl();
            }

            if ((pageFrom == "PollOptions") && (pageTo == "PollOptionsSettings"))
            {
                CurrentPoll = CreatePoll();
                FillOptionList(CurrentPoll.Options);
                PollNameSettingsLiteral.Text = PollingName.Value.Trim();
            }

            base.ActivePageChanged(pageTo, pageFrom);

            //disable controls after base handler
            if (pageTo == "PollOptionsSettings")
                BackButton.Disabled = true;
        }

        /// <summary>
        /// Ends the wizard.
        /// </summary>
        protected override void EndWizard()
        {
            if (ShowPageEditorFunctionality && (CurrentPoll != null))
            {
                Assert.IsNotNullOrEmpty(ParentItem[FieldIDs.LayoutField], String.Format("Item {0} has no layout", ParentItem.Paths.Path));

                LayoutDefinition definition = LayoutDefinition.Parse(ParentItem[FieldIDs.LayoutField]);
                var rendering = new RenderingDefinition();

                rendering.ItemID = PollConstants.PollRenderingID;

                rendering.Placeholder = Context.ClientPage.Request.Form["__LISTACTIVE"];

                rendering.Datasource = String.Format("{0}/{1}", CurrentPoll.InnerItem.Parent.Paths.Path, CurrentPoll.Name);

                definition.GetDevice(PollConstants.PollRenderingDefaultDevice).AddRendering(rendering);
                using (new EditContext(ParentItem))
                {
                    ParentItem[FieldIDs.LayoutField] = definition.ToXml();
                }
            }
            base.EndWizard();
        }

        #endregion override methods

        #region helpers

        private string GetItemName(string itemName)
        {
            if (ItemUtil.GetItemNameError(itemName).Length > 0)
            {
                return ItemUtil.ProposeValidItemName(itemName);
            }
            return itemName;
        }


        /// <summary>
        /// Gets the edit control by ID.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        private Edit GetEditControlByID(string id)
        {
            foreach (Edit pollOption in Parameters.Controls)
            {
                if (pollOption.ID == id)
                    return pollOption;
            }
            return null;
        }

        /// <summary>
        /// Adds the poll option edit control.
        /// </summary>
        /// <returns></returns>
        private string AddPollOptionEditControl()
        {
            var control = new Edit
                               {
                                   ID = Control.GetUniqueID("L"),
                                   Width = Unit.Percentage(100),
                                   Margin = "8px 0 0 0"
                               };
            Context.ClientPage.AddControl(Parameters, control);
            control.Change = string.Format("polloption:namechange(id={0})", control.ID);
            SheerResponse.Insert(Parameters.ID, "beforeEnd", control);
            return control.ID;
        }

        #endregion helpers

        #region message handlers


        /// <summary>
        /// Called when option name changed.
        /// </summary>
        /// <param name="message">The message.</param>
        [HandleMessage("polloption:namechange")]
        protected void OnOptionNameChange(Message message)
        {
            string optionId = message.Arguments["id"];

            Edit lastFilledEdit = Parameters.FindControl(LastFilledId) as Edit;

            if (lastFilledEdit != null)
                if (optionId == LastFilledId && !string.IsNullOrEmpty(lastFilledEdit.Value))
                {
                    LastFilledId = LastId;
                    LastId = AddPollOptionEditControl();
                }
            Edit optionName = GetEditControlByID(optionId);
            if (!Results.Contains(optionId))
            {
                Results.Add(optionId, optionName.Value);
            }
            else
            {
                Results[optionId] = optionName.Value;
            }
            SheerResponse.SetReturnValue(true);
        }

        /// <summary>
        /// Called when poll option selection index changed.
        /// </summary>
        /// <param name="args">The args.</param>
        [HandleMessage("polloption:selectchange", true)]
        protected void OnPollOptionSelectionChanged(ClientPipelineArgs args)
        {
            if (OptionList.SelectedItems.Length != 1)
                return;

            CurrentOption = OptionList.SelectedItems[0].ColumnValues["index"].ToString();
        }

        /// <summary>
        /// Called when poll option settings change clicked.
        /// </summary>
        /// <param name="args">The args.</param>
        [HandleMessage("polloption:settingschange", true)]
        protected void OnPollOptionSettingsChange(ClientPipelineArgs args)
        {
            if (CurrentPoll != null)
            {
                int currentOptionIndex;
                if (!string.IsNullOrEmpty(CurrentOption) && int.TryParse(CurrentOption, out currentOptionIndex))
                {
                    var option = CurrentPoll[currentOptionIndex];
                    var fieldCollection = new NameValueCollection
                                              {                                                 
                                                  {"fields", "__Tracking"},
                                                  { "title", string.Format("Change tracking setting for the poll option: {0}", option.Title) },
                                                  {"icon", option.InnerItem.Appearance.Icon}
                                              };
                    CommandContext commandContext = new CommandContext(option.InnerItem);

                    //start the Field Editor
                    commandContext.Parameters.Add(fieldCollection);
                    var fieldEditor = new PollFieldEditor();
                    fieldEditor.Execute(commandContext);
                }
            }
        }

        /// <summary>
        /// Called when poll settings change clicked.
        /// </summary>
        /// <param name="args">The args.</param>
        [HandleMessage("poll:settingschange", true)]
        protected void OnPollSettingsChange(ClientPipelineArgs args)
        {
            if (CurrentPoll != null)
            {
                var fieldCollection = new NameValueCollection
                                          {
                                              {"fields", "Intro|Thank you|__Tracking"},                                              
                                              {"title", string.Format("Change settings for the poll: {0}", CurrentPoll.Name)},
                                              {"icon", CurrentPoll.InnerItem.Appearance.Icon}
                                          };
                CommandContext commandContext = new CommandContext(CurrentPoll.InnerItem);

                //start the Field Editor
                commandContext.Parameters.Add(fieldCollection);
                var fieldEditor = new PollFieldEditor();
                fieldEditor.Execute(commandContext);
            }
        }
        [HandleMessage("webedit:saved")]
        public void MyHandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");
            base.HandleMessage(message);
            //if (message["id"] == this.ID)
            //{
            //    string javascript = string.Empty;
            //    string name = message.Name;
            //    if (name != null)
            //    {
            //        if (!(name == "contentanalytics:openprofiles"))
            //        {
            //            if (name == "contentanalytics:opengoals")
            //            {
            //                javascript = "window.frames['{0}'].scForm.postRequest('','','','analytics:opengoals')".FormatWith(new object[] { this.ID });
            //            }
            //            else if (name == "contentanalytics:openattributes")
            //            {
            //                javascript = "window.frames['{0}'].scForm.postRequest('','','','analytics:opentrackingfield')".FormatWith(new object[] { this.ID });
            //            }
            //        }
            //        else
            //        {
            //            javascript = "window.frames['{0}'].scForm.postRequest('','','','analytics:openprofiles')".FormatWith(new object[] { this.ID });
            //        }
            //    }
            //    SheerResponse.Eval(javascript);
            //}
        }


        #endregion
    }
}