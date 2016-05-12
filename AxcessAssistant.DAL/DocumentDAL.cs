using System;
using System.Collections.Generic;
using System.Linq;
using AxcessAssistant.DAL.Models;

namespace AxcessAssistant.DAL
{
    public class DocumentDAL
    {
        private static List<Document> _documents;

        public DocumentDAL()
        {
            if(_documents == null)
                init();
        }

        public Document GetDocument(int id)
        {
            return _documents.FirstOrDefault(x => x.ID == id);
        }

        public List<Document> FindDocumentsAllByClientId(int clientId, DocumentType? documentType = null)
        {
            return _documents.FindAll(x => x.ClientID == clientId && (documentType == null || x.DocType == documentType.Value));
        }

        public List<Document> FindDocumentsForClientUpToDate(int clientId, DateTime toDate, DocumentType? documentType = null)
        {
            var documents = FindDocumentsAllByClientId(clientId, documentType);

            if (documents.Count == 0) return documents;

            return documents.Where(x => x.LastModifiedDate <= toDate).OrderByDescending(x => x.LastModifiedDate).ToList();
        }

        private void init()
        {
            _documents = new List<Document>();
            _documents.Add(new Document
            {
                ID = 2,
                LastModifiedDate = new DateTime(2016, 5, 12),
                Name = "123",
                Amount = 975.00m,
                ClientID = 1,
                DocType = DocumentType.Invoice,
                FileName = "Invoice_123.pdf"
            });

            _documents.Add(new Document
            {
                ID = 1,
                LastModifiedDate = new DateTime(2016, 4, 12),
                Name = "122",
                Amount = 1533.00m,
                ClientID = 1,
                DocType = DocumentType.Invoice,
                FileName = "Invoice_122.pdf"
            });
        }
    }
}

