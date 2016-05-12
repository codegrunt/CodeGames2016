using System;
using System.Linq;
using System.Threading.Tasks;
using AxcessAssistant.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace AxcessAssistant.Dialogs
{
    [LuisModel("2e29faee-2608-4fe6-b126-71880faf19e5", "55d649e8b94c41aab7e358cd34c4a978")]
    [Serializable]
    public class BaseDialog : LuisDialog<Entity>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry I did not understand: {String.Join(",", result.Intents.Select(i => i.Intent))}";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Find Intent")]
        public async  Task Find(IDialogContext context, LuisResult result)
        {
            string message =
                $"Recieved Find Intent with the following Entities: {string.Join(",", result.Entities.Select(e => e.Entity + e.Type))}";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Create Intent")]
        public async Task Create(IDialogContext context, LuisResult result)
        {
            string message = $"Recieved Find Intent with the following Entities: {string.Join(",", result.Entities.Select(e => e.Entity + e.Type))}";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        public BaseDialog(ILuisService service = null)
            : base(service)
        {
        }

    }
}