using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxcessAssistant.DAL;
using AxcessAssistant.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace AxcessAssistant.Dialogs
{
    [LuisModel("02853d60-2139-4908-ab6c-5deea7a6fb30", "b33a40080b2d4a2f8bc3ca754e749f7c")]
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
            var noteDiag = new NoteDiag();
            noteDiag.ShowDialog(context);
        }

        [LuisIntent("ReviewNotes")]
        public async  Task ReviewNotes(IDialogContext context, LuisResult result)
        {
            RetrieiveEntities(context,result);
            var noteDiag = new NoteDiag();
            noteDiag.ShowDialog(context);
        }

        [LuisIntent("GetInvoice")]
        public async Task GetInvoice(IDialogContext context, LuisResult result)
        {
            RetrieiveEntities(context, result);
            var invDiag = new InvoiceDiag();
            invDiag.ShowDialog(context);
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

        public async Task StartOver(IDialogContext context)
        {
            await context.PostAsync("Is there anything else I can help you with today?");
            context.Wait(MessageReceived);
        }

        public BaseDialog(ILuisService service = null)
            : base(service)
        {
        }

    }
}