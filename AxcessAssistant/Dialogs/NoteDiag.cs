using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

using AxcessAssistant.DAL.Models;
using AxcessAssistant.DAL;


namespace AxcessAssistant.Dialogs
{
    [Serializable]
    public class NoteDiag : IDialog<object>
    {

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            
            Client clt = null;
            context.ConversationData.TryGetValue("client", out clt);

            var msg = "No note entered.";
            if (!String.IsNullOrEmpty(message.Text))
            {
                msg = "Note created for " + clt.ClientName + " with the value '" + message.Text + "'";
                var cltDal = new ClientDAL();
                cltDal.AddNote(clt.ID, message.Text);
            }

            await context.PostAsync(msg);
            var baseDiag = new BaseDialog();
            context.Wait(baseDiag.StartOver);

        }
    }
}