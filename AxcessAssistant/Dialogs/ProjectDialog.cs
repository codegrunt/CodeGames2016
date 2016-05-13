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
                var project = projects.FirstOrDefault(x => x.Name.Equals(entity.EntityValue, StringComparison.InvariantCultureIgnoreCase));
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

            if (string.Equals(message.Text, "cancel", StringComparison.InvariantCultureIgnoreCase))
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
            var projectDal = new ProjectDAL();
            if (_client == null)
            {
                var cltDAL = new ClientDAL();
                var clts = cltDAL.FindClientsByName(message.Text);
                if (clts != null && clts.Count > 0)
                {
                    _client = clts[0];
                    context.ConversationData.SetValue("client", _client);
                    var projects = projectDal.FindProjectsByClientId(_client.ID);
                    if (projects.Any())
                    {
                        await context.PostAsync($"{_client.ClientName} has the following projects: {string.Join(",", projects.Select(x => x.Name))}");
                        await context.PostAsync("Which project would you like to change the status of?");
                    }
                    else
                    {
                        await context.PostAsync($"There were no projects found for {_client.ClientName}.");
                        if (_contextAction != null)
                        {
                            await _contextAction(context);
                        }
                        else
                        {
                            var bd = new BaseDialog();
                            await bd.StartOver(context);
                        }
                    }
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    await context.PostAsync($"Unable to locate client {message.Text}.");
                    await context.PostAsync("Which client would you like to enter a note for?");
                    context.Wait(MessageReceivedAsync);
                }
            }
            else if (_proj == null)
            {
                var projects = projectDal.FindProjectsByClientId(_client.ID);
                if (projects.Any())
                {
                    var project = projects.FirstOrDefault(x => x.Name.Equals(message.Text, StringComparison.InvariantCultureIgnoreCase));
                    if (project != null)
                    {
                        _proj = project;
                    }
                    else
                    {
                        await context.PostAsync($"Unable to locate project: {message.Text}.");
                        await
                            context.PostAsync(
                                $"{_client.ClientName} has the following projects: {string.Join(",", projects.Select(x => x.Name))}");
                        await context.PostAsync("Which project would you like to change the status of?");
                        context.Wait(MessageReceivedAsync);
                    }
                    await context.PostAsync($"What is the status would you like to set project '{_proj.Name}' to?");
                    context.Wait(MessageReceivedAsync);
                }
                else
                {
                    await context.PostAsync($"There were no projects found for {_client.ClientName}.");
                    if (_contextAction != null)
                    {
                        await _contextAction(context);
                    }
                    else
                    {
                        var bd = new BaseDialog();
                        await bd.StartOver(context);
                    }
                }
            }
            else
            {
                var msg = "No Status provided.  Canceling status change.";
                if (!String.IsNullOrEmpty(message.Text))
                {
                    msg = $"Status of Project {_proj.Name} changed to '{message.Text}' for {_client.ClientName}";
                    _proj.Status = message.Text;
                    projectDal.UpdateProject(_proj);
                }
                await context.PostAsync(msg);
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