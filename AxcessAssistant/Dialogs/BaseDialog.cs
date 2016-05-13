using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.UI.WebControls;
using System.Xml.XPath;
using AxcessAssistant.DAL;
using AxcessAssistant.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace AxcessAssistant.Dialogs
{
    [LuisModel("2e29faee-2608-4fe6-b126-71880faf19e5", "55d649e8b94c41aab7e358cd34c4a978")]
    [Serializable]
    public class BaseDialog : LuisDialog<Entity>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: \"{result.Query}\"";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Create Notes")]
        public async Task CreateNotes(IDialogContext context, LuisResult result)
        {
            RetrieiveEntities(context, result);
            string message =
                $"Recieved Create Notes Intent with the following Entities: {string.Join(",", result.Entities.Select(e => e.Entity + e.Type))}";
            await context.PostAsync(message);
            var invoiceDiag = new InvoiceDiag();
            context.Wait(invoiceDiag.MessageReceivedAsync);
        }

        [LuisIntent("ReviewNotes")]
        public async  Task ReviewNotes(IDialogContext context, LuisResult result)
        {
            RetrieiveEntities(context,result);
            string message =
                $"Recieved ReviewNotes Intent with the following Entities: {string.Join(",", result.Entities.Select(e => e.Entity + e.Type))}";
            await context.PostAsync(message);
            var invoiceDiag  = new InvoiceDiag();
            context.Wait(invoiceDiag.MessageReceivedAsync);
        }

        [LuisIntent("GetInvoice")]
        public async Task GetInvoice(IDialogContext context, LuisResult result)
        {
            RetrieiveEntities(context, result);
            string message = $"Recieved GetInvoice Intent with the following Entities: {string.Join(",", result.Entities.Select(e => e.Entity + e.Type))}";
            await context.PostAsync(message);
            var invoiceDiag = new InvoiceDiag();
            context.Wait(invoiceDiag.MessageReceivedAsync);
        }

        [LuisIntent("Update Intent")]
        public async Task Update(IDialogContext context, LuisResult result)
        {
            RetrieiveEntities(context, result);
            string message = $"Recieved Create Intent with the following Entities: {string.Join(",", result.Entities.Select(e => e.Entity + e.Type))}";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        public void RetrieiveEntities(IDialogContext context, LuisResult luis)
        {
            var clientDal = new ClientDAL();
            Dictionary<string, Func<string, Entity>> dataRetriever = new Dictionary<string, Func<string, Entity>>();
            dataRetriever.Add("Note", x => new Entity {EntityType = EntityType.Note, EntityValue = x});
            dataRetriever.Add("ClientName", x =>
            {
                var clients = clientDal.FindClientsByName(x);
                if (clients.Count > 0)
                {
                    context.ConversationData.SetValue("client", clients[0]);
                }
                return new Entity {EntityType = EntityType.Client, EntityValue = x};
            } );
            dataRetriever.Add("ProjectName", x => new Entity() { EntityType = EntityType.Project, EntityValue = x });
            dataRetriever.Add("InvoiceNumber", x => new Entity() { EntityType = EntityType.Invoice, EntityValue = x });
            dataRetriever.Add("ordinal", x => new Entity() { EntityType = EntityType.Ordinal, EntityValue = x });
            var result = new List<Entity>();
            luis.Entities.ToList().ForEach(x =>
            {
                if (dataRetriever.ContainsKey(x.Type))
                {
                    result.Add(dataRetriever[x.Type](x.Entity));
                }
            });
            context.ConversationData.SetValue("entities", result);
        } 

        public Task StartOver(IDialogContext context, IAwaitable<Message> message)
        {
            return MessageReceived(context, message);
        }

        public BaseDialog(ILuisService service = null)
            : base(service)
        {
        }

    }
}