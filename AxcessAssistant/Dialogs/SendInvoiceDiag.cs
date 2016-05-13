using System;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using System.Linq;
using AxcessAssistant.DAL.Models;
using AxcessAssistant.DAL;

namespace AxcessAssistant.Dialogs
{
    [Serializable]
    public class SendInvoiceDiag : IDialog<object>, IAxcessDiag
    {
        private int invoiceId = 0;
        private Client client = null;
        private Document _doc;
        private Func<IDialogContext, Task> _contextAction;

        public Task StartAsync(IDialogContext context, Func<IDialogContext, Task> contextAction)
        {
            _contextAction = contextAction;
            return StartAsync(context);
        }
        public async Task StartAsync(IDialogContext context)
        {
            if (client == null)
                context.ConversationData.TryGetValue("client", out client);
            context.ConversationData.TryGetValue("invoice", out invoiceId);
            if (client == null)
            {
                await context.PostAsync("Which client would you like to send the invoice to?");
                context.Wait(MessageReceivedAsync);
            }
            else if (invoiceId != default(int))
            {
                DocumentDAL documentDal = new DocumentDAL();
                var invoice = documentDal.GetDocument(invoiceId);
                if(invoice != null)
                {
                    await context.PostAsync($"Sent invoice {invoice.ID} to {client.ContactName} at {client.Email}");
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
                else
                {
                    var documents = documentDal.FindDocumentsAllByClientId(client.ID);
                    if (!documents.Any())
                    {
                        await context.PostAsync("Client does not have any invoices");
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
                    await context.PostAsync($"{client.ClientName} has the following invoices: {string.Concat(",", documents.Select(x => x.ID))}");
                    await context.PostAsync("Which Invoice do you want?");
                    context.Wait(MessageReceivedAsync);
                }
                
            }           
        }
        
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            string msg = "Please choose a client to send the invoice to.";
            string msg2 = string.Empty;

            var documentDal = new DocumentDAL();
            var inv = documentDal.GetDocument(invoiceId);
            if (inv != null)
            {
                _doc = inv;
                msg = $"Here's invoice {inv.Name}.";
                msg2 = $"https://axcessassistantbot.azurewebsites.net/api/files?fileName={inv.FileName}";
            }

            else if (client != null)
            {
                msg = $"I can't find any invoices for {client.ClientName}";
            }
            await context.PostAsync(msg);
            if (!string.IsNullOrEmpty(msg2))
            {
                await context.PostAsync(msg2);
            }
                       

            if (_doc != null)
            {
                msg = "Sent invoice to " + message.Text;
                await context.PostAsync(msg);
                Reset();
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
            else
            {
                var clt = GetClient(message.Text);
                if (clt != null)
                {
                    context.ConversationData.SetValue("client", clt);
                    //await CheckClient(context, clt);
                }
                else
                {
                    msg = $"Can not find client {message.Text}.";
                    await context.PostAsync(msg);
                }

                context.Wait(MessageReceivedAsync);
            }
        }

        private bool Reset()
        {
            _doc = null;
            return true;
        }


        private Client GetClient(string clientName)
        {
            var clts = new ClientDAL();
            var clt = clts.FindClientsByName(clientName);
            return clt[0];
        }
    }
}