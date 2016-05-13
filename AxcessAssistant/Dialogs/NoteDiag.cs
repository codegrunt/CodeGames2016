using System;
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

            var msg = "Which client would you like to enter a note for?";

            if (clt != null)
            {
                msg = $"Please enter note for client {clt.ClientName}.";
            }

            await context.PostAsync(msg);
            context.Wait(MessageReceivedAsync);
        }

        private string _note;

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            Client clt = null;
            context.ConversationData.TryGetValue("client", out clt);

            if (clt == null)
            {
                var cltDAL = new ClientDAL();
                var clts = cltDAL.FindClientsByName(message.Text);
                if (clts != null && clts.Count > 0)
                {
                    clt = clts[0];
                    context.ConversationData.SetValue("client", clt);
                    var msg = $"What would you like the note for {clt.ClientName} to say?";
                    await context.PostAsync(msg);
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    await context.PostAsync($"Unable to locate client {message.Text}.");
                    await context.PostAsync("Which client would you like to enter a note for?");
                    context.Wait(MessageReceivedAsync);
                }
            }
            else
            {
                var msg = "No note entered.";
                if (!String.IsNullOrEmpty(message.Text))
                {
                    msg = $"Note '{message.Text}' created for {clt.ClientName}";
                    var cltDal = new ClientDAL();
                    cltDal.AddNote(clt.ID, message.Text);
                }

                await context.PostAsync(msg);
                if (_contextAction != null)
                {
                    await _contextAction(context);
                }
                else
                {
                    var baseDiag = new BaseDialog();
                    await baseDiag.StartOver(context);
                }
            }
        }
    }
}