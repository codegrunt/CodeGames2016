using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxcessAssistant.DAL;
using AxcessAssistant.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace AxcessAssistant.Dialogs
{
    [LuisModel("0131b28f-6dbe-4fba-83fd-51ce8555cb54", "55d649e8b94c41aab7e358cd34c4a978")]
    [Serializable]
    public class BaseDialog : LuisDialog<Entity>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = result.Query.ToLower().Contains("thank you") ? $"It was a pleasure, I am always here to help." : $"Sorry I did not understand: \"{result.Query}\".";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("CreateNote")]
        public async Task CreateNotes(IDialogContext context, LuisResult result)
        {
            RetrieiveEntities(context, result);
            var noteDiag = new NoteDiag();
            await noteDiag.StartAsync(context, StartOver);
        }

        [LuisIntent("GetNote")]
        public async Task ReviewNotes(IDialogContext context, LuisResult result)
        {
            RetrieiveEntities(context, result);
            var noteDiag = new ShowNotesDiag();
            await noteDiag.StartAsync(context, StartOver);
        }

        [LuisIntent("GetInvoice")]
        public async Task GetInvoice(IDialogContext context, LuisResult result)
        {
            RetrieiveEntities(context, result);
            var invDiag = new GetInvoiceDiag();
            await invDiag.StartAsync(context, StartOver);
        }

        [LuisIntent("SendInvoice")]
        public async Task SendInvoice(IDialogContext context, LuisResult result)
        {
            RetrieiveEntities(context, result);
            var invDiag = new GetInvoiceDiag();
            await invDiag.StartAsync(context, StartOver);
        }

        [LuisIntent("UpdateProject")]
        public async Task Update(IDialogContext context, LuisResult result)
        {
            RetrieiveEntities(context, result);
            var projDiag = new ProjectDialog();
            await projDiag.StartAsync(context, StartOver);
        }

        public void RetrieiveEntities(IDialogContext context, LuisResult luis)
        {
            var clientDal = new ClientDAL();
            Dictionary<string, Func<string, Entity>> dataRetriever = new Dictionary<string, Func<string, Entity>>();
            dataRetriever.Add("Note", x => new Entity { EntityType = EntityType.Note, EntityValue = x });
            dataRetriever.Add("ClientName", x =>
            {
                var clients = clientDal.FindClientsByName(x);
                if (clients.Count > 0)
                {
                    context.ConversationData.SetValue("client", clients[0]);
                    context.ConversationData.RemoveValue("invoice");
                }
                return new Entity { EntityType = EntityType.Client, EntityValue = x };
            });
            dataRetriever.Add("ProjectName", x => new Entity() { EntityType = EntityType.Project, EntityValue = x });
            dataRetriever.Add("InvoiceNumber", x => new Entity() { EntityType = EntityType.Invoice, EntityValue = x });
            dataRetriever.Add("Ordinal", x => new Entity() { EntityType = EntityType.Ordinal, EntityValue = x });
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