using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.UI;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.WebEdit;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Text;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.Modules.DMSPoll.Commands
{
    class PollFieldEditor : FieldEditorCommand
    {
        /// <summary>
        /// Gets the configured Field Editor options. Options determine both the look of Field Editor and the actual fields available for editing.
        /// </summary>
        /// <param name="args">The pipeline arguments. Current item URI is accessible as 'uri' parameter</param>
        /// <param name="form">The form values.</param>
        /// <returns>The options.</returns>        
        protected override PageEditFieldEditorOptions GetOptions(ClientPipelineArgs args, NameValueCollection form)
        {
            List<FieldDescriptor> fieldDescriptors = new List<FieldDescriptor>();
            Item item = Database.GetItem(ItemUri.Parse(args.Parameters["uri"]));
            Assert.IsNotNull(item, "item");
            string fields = args.Parameters["fields"];
            Assert.IsNotNullOrEmpty(fields, "Field Editor command expects 'fields' parameter");
            string title = args.Parameters["title"];
            Assert.IsNotNullOrEmpty(title, "Field Editor command expects 'title' parameter");
            string icon = args.Parameters["icon"];
            Assert.IsNotNullOrEmpty(icon, "Field Editor command expects 'icon' parameter");

            foreach (string field in new ListString(fields))
            {
                if (item.Fields[field] != null)
                {
                    fieldDescriptors.Add(new FieldDescriptor(item, field));
                }
            }
            PageEditFieldEditorOptions options = new PageEditFieldEditorOptions(form, fieldDescriptors);
            options.Title = title;
            options.Icon = icon;
            return options;

        }


        /// <summary>
        /// Sheer UI processor methods that orchestrates starting the Field Editor and processing the returned value
        /// </summary>
        /// <param name="args">The arguments.</param>
        protected new void StartFieldEditor(ClientPipelineArgs args)
        {

            HttpContext current = HttpContext.Current;

            if (current != null)
            {

                Page handler = current.Handler as Page;

                if (handler != null)
                {

                    NameValueCollection form = handler.Request.Form;

                    if (form != null)
                    {

                        if (!args.IsPostBack)
                        {

                            SheerResponse.ShowModalDialog(this.GetOptions(args, form).ToUrlString().ToString(), "720", "480", string.Empty, true);

                            args.WaitForPostBack();



                        }

                        else if (args.HasResult)
                        {

                            foreach (FieldDescriptor fd in PageEditFieldEditorOptions.Parse(args.Result).Fields)
                            {

                                Item item = Context.ContentDatabase.GetItem(fd.ItemUri.ToDataUri());

                                if (item.Fields[fd.FieldID].Name == "__Tracking")
                                {

                                    continue;

                                }

                                using (new SecurityDisabler())
                                {

                                    using (new EditContext(item))
                                    {

                                        item[fd.FieldID] = fd.Value;

                                    }

                                }

                            }

                        }

                    }

                }

            }

        }




        //********************************************************    End block    *************************************************
    }
}
