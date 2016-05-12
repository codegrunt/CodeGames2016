using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;


namespace AxcessAssistant.Forms
{
    [Serializable]
    public class Invoice
    {
        public string InvoiceNumber;
        public string ClientId;

        public static IForm<Invoice> BuildForm()
        {
            return new FormBuilder<Invoice>().Message("Invoice retrieval bot").Build();
        }
    }
}