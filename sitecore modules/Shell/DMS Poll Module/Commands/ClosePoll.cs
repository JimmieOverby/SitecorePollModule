using Sitecore.Shell.Framework.Commands;
using Sitecore.Modules.DMSPoll.Domain;
using Sitecore.Data.Items;

namespace Sitecore.Modules.DMSPoll.Commands
{
   public class ClosePoll : Command
   {

      public override void Execute(CommandContext context)
      {
         PollItem pollItem = new PollItem(context.Items[0]);
         pollItem.IsClosed = true;
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

         PollItem pollItem = new PollItem(context.Items[0]);
         if (pollItem.IsClosed)
         {
            return CommandState.Disabled;
         }

         if (pollItem.IsArchived)
         {
            return CommandState.Disabled;
         }

         return CommandState.Enabled;
      }
   }
}
