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
        Task StartAsync(IDialogContext context, Func<IDialogContext, Task> contextAction);

        Task MessageReceivedAsync(IDialogContext context, IAwaitable<Message> argument);
    }
}
