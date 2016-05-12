using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;

using AxcessAssistant.DAL;
using AxcessAssistant.DAL.Models;


namespace AxcessAssistant.Forms
{
    [Serializable]
    public class Invoice
    {
        private Document _invoice;

        private int _clientId;
        public int ClientId {
            get
            {
                return _clientId;
            }
            set
            {

                var clts = new ClientDAL();
                var clt = clts.GetClientById(value);
                if (clt != null)
                {
                    var docs = new DocumentDAL();
                    var doc = docs.FindDocumentsForClientUpToDate(clt.ID, DateTime.Now, DocumentType.Invoice);
                    if (doc != null || doc.Count > 0)
                    {
                        _invoice = doc[0];
                        _clientId = value;
                        Console.WriteLine("found it");
                    }
                }
            } }

        public static IForm<Invoice> BuildForm()
        {
            return new FormBuilder<Invoice>().Message("Invoice retrieval bot").Build();
        }
    }
}