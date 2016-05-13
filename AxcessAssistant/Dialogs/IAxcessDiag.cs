using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

namespace AxcessAssistant.Dialogs
{
    interface IAxcessDiag
    {
        void ShowDialog(IDialogContext context);

        Task StartAsync(IDialogContext context);

        Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument);
    }
}
