using System;
using System.Linq;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using Sitecore.Web.UI.Sheer;
using Sitecore.Shell.Framework.Pipelines;
using Sitecore.Data;
using Sitecore.Pipelines.GetRenderingDatasource;

namespace Sitecore.Modules.DMSPoll.Domain
{
    public class PollEventHandler
    {
        // Methods
        public bool CheckIsPollItemNameDuplicate(string pollName)
        {
            PollAnalyticsDataProvider provider = new PollAnalyticsDataProvider();
            return (provider.PollExists(pollName) || (this.CountPollItemNameDuplicates(pollName) > 0));
            return (this.CountPollItemNameDuplicates(pollName) > 0);
        }

        public void CheckPollCloneDestination(CopyItemsArgs args)
        {
            string str = args.Parameters[1];
            if (!string.IsNullOrEmpty(str))
            {
                Item item = Context.ContentDatabase.Items.GetItem(new ID(str));
                if ((item != null) && (item.TemplateID == PollConstants.PollTemplateID))
                {
                    Context.ClientPage.ClientResponse.Alert("Can't clone DMS Poll items.");
                    args.AbortPipeline();
                }
            }
        }

        public int CountItemPathDuplicates(Item item)
        {
            return item.Parent.Children.Where<Item>(delegate(Item x)
            {
                return (x.Name == item.Name);
            }).Count<Item>();
        }

        public int CountPollItemNameDuplicates(string pollName)
        {
            using (new SecurityDisabler())
            {
                if (PollConstants.DatabaseContext != null)
                {
                    Item item = PollConstants.DatabaseContext.GetItem("/sitecore/content");
                    if (item != null)
                    {
                        string str = string.Format("@@templateid='{0}' and @@name='{1}'", PollConstants.PollTemplateID, pollName);
                        Item[] itemArray = item.Axes.SelectItems(string.Format(".//*[{0}]", str));
                        if (itemArray != null)
                        {
                            return itemArray.Length;
                        }
                    }
                }
            }
            return 0;
        }

        public void DeletePollClones(CopyItemsArgs args)
        {
            if (args.Copies != null)
            {
                foreach (Item item in args.Copies.Where<Item>(delegate(Item x)
                {
                    return x != null;
                }))
                {
                    if (item == null)
                    {
                        break;
                    }
                    //for (Item item2 = item.Axes.GetDescendants().Where<Item>(delegate (Item x) {
                    //    return (x.TemplateID == PollConstants.PollTemplateID);
                    //}).FirstOrDefault<Item>(); item2 != null; item2 = item.Axes.GetDescendants().Where<Item>(CS$<>9__CachedAnonymousMethodDelegate5).FirstOrDefault<Item>())
                    //{
                    //    using (new SecurityDisabler())
                    //    {
                    //        item2.Delete();
                    //    }
                    //    if (CS$<>9__CachedAnonymousMethodDelegate5 == null)
                    //    {
                    //        CS$<>9__CachedAnonymousMethodDelegate5 = delegate (Item x) {
                    //            return x.TemplateID == PollConstants.PollTemplateID;
                    //        };
                    //    }
                    //}
                }
            }
        }

        public void LaunchDmsPollWizard(GetRenderingDatasourceArgs args)
        {
            if (((args != null) && (args.RenderingItem != null)) && (args.RenderingItem.ID.ToString() == PollConstants.PollRenderingID))
            {
                Item item = args.ContentDatabase.Items.GetItem(args.ContextItemPath);
                if (item != null)
                {
                    string str = string.Format("/sitecore/shell/default.aspx?xmlcontrol=DMSPollWizard&id={0}&language={1}&showpageeditorfunctionality=False", item.ID, item.Language.Name);
                    args.DialogUrl = str;
                }
            }
        }

        public void OnItemCopied(object sender, EventArgs args)
        {
            if ((args is SitecoreEventArgs) && (Event.ExtractParameter(args, 0) is Item))
            {
                Item item2 = Event.ExtractParameter(args, 1) as Item;
                if (item2 != null)
                {
                    //for (Item item3 = item2.Axes.GetDescendants().Where<Item>(delegate (Item x) {
                    //    return (x.TemplateID == PollConstants.PollTemplateID);
                    //}).FirstOrDefault<Item>(); item3 != null; item3 = item2.Axes.GetDescendants().Where<Item>(CS$<>9__CachedAnonymousMethodDelegate9).FirstOrDefault<Item>())
                    //{
                    //    using (new SecurityDisabler())
                    //    {
                    //        item3.Delete();
                    //    }
                    //    if (CS$<>9__CachedAnonymousMethodDelegate9 == null)
                    //    {
                    //        CS$<>9__CachedAnonymousMethodDelegate9 = delegate (Item x) {
                    //            return x.TemplateID == PollConstants.PollTemplateID;
                    //        };
                    //    }
                    //}
                }
            }
        }

        public void OnItemCopying(object sender, EventArgs args)
        {
            SitecoreEventArgs args2 = args as SitecoreEventArgs;
            if (args2 != null)
            {
                Item item = Event.ExtractParameter(args, 0) as Item;
                if ((item != null) && (item.TemplateID == PollConstants.PollTemplateID))
                {
                    args2.Result.Cancel = true;
                    Context.ClientPage.ClientResponse.Alert("Can't copy DMS Poll items.");
                }
            }
        }

        public void OnItemSaved(object sender, EventArgs args)
        {
            if (args != null)
            {
                Item item = Event.ExtractParameter(args, 0) as Item;
                if ((item != null) && (item.TemplateID == PollConstants.PollTemplateID))
                {
                    bool flag = this.CountPollItemNameDuplicates(item.Name) > 1;
                    bool flag2 = this.CountItemPathDuplicates(item) > 1;
                    if (flag || flag2)
                    {
                        string str;
                        int num = 0;
                        do
                        {
                            num++;
                            str = string.Format("{0} {1} {2}", Translate.Text("Copy of"), item.Name, num).Replace(' ', '_');
                        }
                        while (this.CountPollItemNameDuplicates(str) > 0);
                        using (new SecurityDisabler())
                        {
                            item.Editing.BeginEdit();
                            try
                            {
                                item.Name = str;
                                item.Editing.EndEdit();
                            }
                            catch (Exception)
                            {
                                item.Editing.CancelEdit();
                            }
                        }
                    }
                }
            }
        }
    }

}
