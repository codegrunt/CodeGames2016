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
    public class ShowNotesDiag : IDialog<object>, IAxcessDiag
    {
        private Func<IDialogContext, Task> _contextAction;

        public Task StartAsync(IDialogContext context, Func<IDialogContext, Task> contextAction)
        {
            _contextAction = contextAction;
            return StartAsync(context);
        }

        public async Task StartAsync(IDialogContext context)
        {

            Client clt = null;
            context.ConversationData.TryGetValue("client", out clt);

            if (clt != null)
            {
                await ShowNotes(context, clt);
                if (_contextAction != null)
                {
                    await _contextAction(context);
                }
                else
                {
                    var bDiag = new BaseDialog();
                    await bDiag.StartOver(context);
                }
            }
            else
            {
                await context.PostAsync("Which client would you like to pull notes for?");
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ShowNotes(IDialogContext context, Client clt)
        {
            var cltDal = new ClientDAL();
            var noteClient = cltDal.GetClientById(clt.ID);

            if (noteClient != null && noteClient.Notes != null && noteClient.Notes.Count > 0)
            {
                await context.PostAsync($"Found the following notes for {noteClient.ClientName}:");
                foreach (var note in noteClient.Notes)
                {
                    await context.PostAsync(note.Text);
                }

            }
            else
            {
                context.ConversationData.RemoveValue("client");
                await context.PostAsync($"Error finding client notes {clt.ClientName}, resetting to main.");
            }
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            var cltDAL = new ClientDAL();
            var clts = cltDAL.FindClientsByName(message.Text);
            if (clts != null && clts.Count > 0)
            {
                var clt = clts[0];

                await ShowNotes(context, clt);
                context.ConversationData.SetValue("client", clt);
                if (_contextAction != null)
                {
                    await _contextAction(context);
                }
                else
                {
                    var bDiag = new BaseDialog();
                    await bDiag.StartOver(context);
                }
            }
            else
            {
                await context.PostAsync($"Could not find client {message.Text}.  Or client did not have any notes.  Please enter another client.");
                context.Wait(MessageReceivedAsync);
            }


        }
    }
}