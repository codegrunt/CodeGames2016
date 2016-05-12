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
    public class InvoiceDiag : IDialog<object>
    {

        private int _clientId;
        private Document _doc;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (message.Text == "reset")
            {
                Reset();
                var msg = "Session reset";
                await context.PostAsync(msg);
                context.Wait(MessageReceivedAsync);
            }
            else if (_doc == null)
            {
                var msg = "Can not find client " + message.Text;
                var msg2 = string.Empty;
                try
                {
                    var cltId = Convert.ToInt32(message.Text);
                    var clt = GetClient(cltId);
                    if (clt != null)
                    {

                        var inv = GetInvoiceByClient(clt.ID);

                        if (inv != null)
                        {
                            _doc = inv;
                            msg = "Found invoice " + inv.Name;
                            msg2 = "Who would you like to send this to?";
                        }
                        else
                            msg = "Can not find invoices for client " + message.Text;
                    }
                }
                catch
                {
                    msg = "Can not find client " + message.Text;
                }
                finally
                {
                    await context.PostAsync(msg);
                    if (msg2 != string.Empty)
                        await context.PostAsync(msg2);
                    context.Wait(MessageReceivedAsync);
                }
            }
            else
            {
                var msg = "Sent invoice to " + message.Text;
                await context.PostAsync(msg);
                Reset();
                context.Wait(MessageReceivedAsync);
            }

        }

        private bool Reset()
        {
            _doc = null;
            return true;
        }


        private Client GetClient(int clientId)
        {
            _clientId = clientId;
            var clts = new ClientDAL();
            var clt = clts.GetClientById(clientId);
            return clt;
        }
        private Document GetInvoiceByClient(int clientId)
        {
            _clientId = clientId;
            var clts = new ClientDAL();
            var clt = clts.GetClientById(clientId);
            if (clt != null)
            {
                var docs = new DocumentDAL();
                var doc = docs.FindDocumentsForClientUpToDate(clt.ID, DateTime.Now, DocumentType.Invoice);
                if (doc != null || doc.Count > 0)
                {
                    return doc[0];
                }
            }
            return null;
        }
    }
}