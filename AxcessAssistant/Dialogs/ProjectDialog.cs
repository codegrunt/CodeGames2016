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
    public class ProjectDialog : IDialog<object>, IAxcessDiag
    {

        private Project _proj;
        private Client _client;
        private Func<IDialogContext, Task> _contextAction;

        public Task StartAsync(IDialogContext context, Func<IDialogContext, Task> contextAction)
        {
            _contextAction = contextAction;
            return StartAsync(context);
        }

        public async Task StartAsync(IDialogContext context)
        {
            var message = $"Please Provide a client";
            var message2 = string.Empty;
            context.ConversationData.TryGetValue("client", out _client);
            var projectDal = new ProjectDAL();

            List<Entity> entities;
            context.ConversationData.TryGetValue("entities", out entities);
            if (entities.Any(x => x.EntityType == EntityType.Project) && _client != null)
            {
                var entity = entities.First(x => x.EntityType == EntityType.Project);
                var projects = projectDal.FindProjectsByClientId(_client.ID);
                var project = projects.FirstOrDefault(x => x.Name == entity.EntityValue);
                if (project != null)
                {
                    _proj = project;
                }
            }
            if (_proj != null)
            {
                message = "What status would you like to set the project to?";
            }
            else if (_client != null)
            {
                var projects = projectDal.FindProjectsByClientId(_client.ID);
                if (projects.Any())
                {
                    message =
                        $"{_client.ClientName} has the following projects: {string.Join(",", projects.Select(x => x.Name))}";
                    message2 =
                        "Which project would you like to change the status of?";
                }
            }
            else
            {
                message = "Please provide a client to change the stutus on a project";
            }
            await context.PostAsync(message);
            if (!string.IsNullOrEmpty(message2))
            {
                await context.PostAsync(message2);
            }
            context.Wait(MessageReceivedAsync);
        }

        

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument)
        {
            var message = await argument;

            if (string.Equals(message.Text, "quit", StringComparison.InvariantCultureIgnoreCase))
            {
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
            else if (message.Text == "reset")
            {
                Reset();
                var msg = "Session reset";
                await context.PostAsync(msg);
                context.Wait(MessageReceivedAsync);
            }
            //else if (_doc == null)
            //{
            //    var msg = "Can not find client " + message.Text;
            //    var msg2 = string.Empty;
            //    try
            //    {
            //        var clt = GetClient(message.Text);
            //        if (clt != null)
            //        {
            //            context.ConversationData.SetValue("client", clt);

            //            var inv = GetInvoiceByClient(clt.ID);

            //            if (inv != null)
            //            {
            //                _doc = inv;
            //                msg = "Found invoice " + inv.Name;
            //                msg2 = "Who would you like to send this to?";
            //            }
            //            else
            //                msg = "Can not find invoices for client " + message.Text;
            //        }
            //    }
            //    catch
            //    {
            //        msg = "Can not find client " + message.Text;
            //    }
            //    finally
            //    {
            //        await context.PostAsync(msg);
            //        if (msg2 != string.Empty)
            //            await context.PostAsync(msg2);
            //        context.Wait(MessageReceivedAsync);
            //    }
            //}
            else
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

        }

        private bool Reset()
        {
            _client = null;
            _proj = null;
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
            if (doc != null || doc.Count > 0)
            {
                return doc[0];
            }
            return null;
        }
    }
}