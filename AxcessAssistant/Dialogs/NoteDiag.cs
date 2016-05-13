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
    public class NoteDiag : IDialog<object>, IAxcessDiag
    {

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async void ShowDialog(IDialogContext context)
        {
            await context.PostAsync("Notes dialog");
            context.Wait(MessageReceivedAsync);
        }

        private string _note;

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;
            
            Client clt = null;
            context.ConversationData.TryGetValue("client", out clt);

            if (clt == null && String.IsNullOrEmpty(_note))
            {
                _note = message.Text;
                var msg = "Which client do you want to add the note too?";
                await context.PostAsync(msg);
                context.Wait(MessageReceivedAsync);
            }
            else if( clt == null && !String.IsNullOrEmpty(_note))
            {
                var cltDAL = new ClientDAL();
                var clts = cltDAL.FindClientsByName(message.Text);
                if (clts != null && clts.Count > 0)
                {
                    clt = clts[0];
                    context.ConversationData.SetValue("client", clt);
                    cltDAL.AddNote(clt.ID, _note);
                    var msg = "Note created for " + clt.ClientName + " with the value '" + _note + "'";
                    await context.PostAsync(msg);
                    var baseDiag = new BaseDialog();
                    await baseDiag.StartOver(context);
                }
            }
            else
            {
                var msg = "No note entered.";
                if (!String.IsNullOrEmpty(message.Text))
                {
                    msg = "Note created for " + clt.ClientName + " with the value '" + message.Text + "'";
                    var cltDal = new ClientDAL();
                    cltDal.AddNote(clt.ID, message.Text);
                }

                await context.PostAsync(msg);
                var baseDiag = new BaseDialog();
                await baseDiag.StartOver(context);
            }

        }
    }
}