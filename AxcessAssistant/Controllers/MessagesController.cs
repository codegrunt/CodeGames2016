using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AxcessAssistant.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Newtonsoft.Json;
using AxcessAssistant.Dialogs;
using AxcessAssistant.Models;

namespace AxcessAssistant
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Message> Post([FromBody]Message message)
        {
            if (message.Type == "Message")
            {
                //return await Conversation.SendAsync(message, () => new InvoiceDiag());
                return await Conversation.SendAsync(message, MakeRootDialog);
            }
            else
            {
                return HandleSystemMessage(message);
            }
        }

        internal static IDialog<Entity> MakeRootDialog()
        {
            return Chain.From(() => new BaseDialog());
        }
            

        private Message HandleSystemMessage(Message message)
        {
            if (message.Type == "Ping")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Ping";
                return reply;
            }
            else if (message.Type == "DeleteUserData")
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == "BotAddedToConversation")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Message";
                reply.Text = "I am Axcess Assistant. How can I assist?";
                return reply;
            }
            else if (message.Type == "BotRemovedFromConversation")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Message";
                reply.Text = "Thanks for using Axcess Assistant.";
                return reply;
            }
            else if (message.Type == "UserAddedToConversation")
            {
            }
            else if (message.Type == "UserRemovedFromConversation")
            {
            }
            else if (message.Type == "EndOfConversation")
            {
                Message reply = message.CreateReplyMessage();
                reply.Type = "Message";
                reply.Text = "Thanks for using Axcess Assistant.";
                return reply;
            }

            return null;
        }
    }
}