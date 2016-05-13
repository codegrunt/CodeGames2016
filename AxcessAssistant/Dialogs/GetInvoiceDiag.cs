using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using AxcessAssistant.DAL.Models;
using AxcessAssistant.DAL;
using AxcessAssistant.Models;

namespace AxcessAssistant.Dialogs
{
    [Serializable]
    public class GetInvoiceDiag : IDialog<object>, IAxcessDiag
    {
        
        private Func<IDialogContext, Task> _contextAction;
        

        public Task StartAsync(IDialogContext context, Func<IDialogContext, Task> contextAction)
        {
            _contextAction = contextAction;
            return StartAsync(context);
        }
        public async Task StartAsync(IDialogContext context)
        {
            DocumentDAL _documentDAL = new DocumentDAL();

            Client clt = null;
            context.ConversationData.TryGetValue("client", out clt);

            List<Entity> entities;
            context.ConversationData.TryGetValue("entities", out entities);
            Document invoice = null;

            if (clt == null)
            {
                await context.PostAsync("Which client would you like to search invoices for?");
                context.Wait(MessageReceivedAsync);
            }
            else if(entities?.Any(x => x.EntityType == EntityType.Ordinal) ?? false)
            {
                var invoices = _documentDAL.FindDocumentsAllByClientId(clt.ID, DocumentType.Invoice).OrderBy(x => x.LastModifiedDate).ToList();

                var ordinal = entities.Find(x => x.EntityType == EntityType.Ordinal);

                if (ordinal.EntityValue.Equals("first", StringComparison.InvariantCultureIgnoreCase))
                {
                    invoice = invoices[0];
                }
                else if (ordinal.EntityValue.Equals("last", StringComparison.InvariantCultureIgnoreCase))
                {
                    invoice = invoices[invoices.Count - 1];
                }
                else if (ordinal.EntityValue.Equals("previous", StringComparison.InvariantCultureIgnoreCase) 
                    || ordinal.EntityValue.Equals("before", StringComparison.InvariantCultureIgnoreCase))
                {
                    Document currentInvoice = invoices[invoices.Count - 1];
                    string invoiceID;
                    context.ConversationData.TryGetValue("invoice", out invoiceID);

                    if (!string.IsNullOrEmpty(invoiceID))
                    {
                        var i = invoices.Find(x => x.ID == int.Parse(invoiceID));
                        if (i != null)
                            currentInvoice = i;
                    }

                    invoice = invoices[Math.Max(0, invoices.IndexOf(currentInvoice)-1)];
                }

                var msg = new Message();

                if (invoice != null)
                {
                    msg.Text = $"Sure, here's invoice {invoice.Name}.";
                    msg.Attachments = new List<Attachment>
                    {
                        new Attachment
                        {
                            Title = invoice.FileName,
                            ContentType = "application/json", ContentUrl = $"https://axcessassistantbot.azurewebsites.net/api/files?fileName={invoice.FileName}"
                        }
                    };
                    context.ConversationData.SetValue("invoice", invoice.ID);
                }
                else
                {
                    msg.Text = $"I can't find any invoices for {clt.ClientName}";
                }
                
                await context.PostAsync(msg);
                context.Wait(MessageReceivedAsync);
            }
            else
            {
                if (entities.Any(x => x.EntityType == EntityType.Invoice))

                    if (!entities.Any(x => x.EntityType == EntityType.Invoice))
                    {
                        var documentDal = new DocumentDAL();
                        var documents = documentDal.FindDocumentsAllByClientId(clt.ID);
                        var msg = $"what invoice do you want for {clt.ClientName}?";
                        var msg2 = $"the available invoices are: {string.Join(",", documents.Select(x => x.ID))}";
                        await context.PostAsync(msg);
                        await context.PostAsync(msg2);
                        context.Wait(MessageReceivedAsync);
                    }
            }
        }
        
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            Client clt = null;
            context.ConversationData.TryGetValue("client", out clt);
            context.Wait(MessageReceivedAsync);
            if (_contextAction != null)
            {
                await _contextAction(context);
            }
            else
            {
                var baseDiag = new BaseDialog();
                await baseDiag.StartOver(context);
            }

            // var message = await argument;

            //if (_doc != null)
            //{
            //    var msg = "Sent invoice to " + message.Text;
            //    await context.PostAsync(msg);
            //    Reset();
            //    if (_contextAction != null)
            //    {
            //        await _contextAction(context);
            //    }
            //    else
            //    {
            //        var baseDiag = new BaseDialog();
            //        await baseDiag.StartOver(context);
            //    }

            //}
            //else
            //{
            //    var clt = GetClient(message.Text);
            //    if (clt != null)
            //    {
            //        context.ConversationData.SetValue("client", clt);
            //        await CheckClient(context, clt);
            //    }
            //    else
            //    {
            //        var msg = $"Can not find client {message.Text}.";
            //        await context.PostAsync(msg);
            //    }

            //    context.Wait(MessageReceivedAsync);
            //}
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