using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class InvoiceDiag : IDialog<object>, IAxcessDiag
    {
        
        private Document _doc;
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
                await CheckClient(context, clt);
            }
            else
            {
                await context.PostAsync("Which client would you like to search invoices for?");
            }
            context.Wait(MessageReceivedAsync);

        }

        private async Task CheckClient(IDialogContext context, Client clt)
        {
            var msg = string.Empty;
            var msg2 = string.Empty;

            var inv = GetInvoiceByClient(clt.ID);
            if (inv != null)
            {
                _doc = inv;
                msg = $"Found invoice {inv.Name}.";
                msg2 = "Who would you like to send this to?";
            }
            else
            {
                msg = $"Can not find invoices for {clt.ClientName}";
                msg2 = "Please enter another client.";
                context.ConversationData.RemoveValue("client");
            }

            await context.PostAsync(msg);
            if (msg2 != string.Empty)
                await context.PostAsync(msg2);
        }
        
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (_doc != null)
            {
                var msg = "Sent invoice to " + message.Text;
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
                    await CheckClient(context, clt);
                }
                else
                {
                    var msg = $"Can not find client {message.Text}.";
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
        private Document GetInvoiceByClient(int clientId)
        {
            var docs = new DocumentDAL();
            var doc = docs.FindDocumentsForClientUpToDate(clientId, DateTime.Now, DocumentType.Invoice);
            if (doc != null && doc.Count > 0)
            {
                return doc[0];
            }
            return null;
        }
    }
}