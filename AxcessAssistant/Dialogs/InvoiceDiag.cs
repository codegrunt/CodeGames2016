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


            var msg = "Can not find client " + message.Text;
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
                context.Wait(MessageReceivedAsync);
            }

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