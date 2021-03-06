﻿using CosmosDbExplorer.Infrastructure.Models;
using CosmosDbExplorer.ViewModel;
using Microsoft.Azure.Documents;

namespace CosmosDbExplorer.Messages
{
    public class OpenImportDocumentViewMessage : OpenTabMessageBase<CollectionNodeViewModel>
    {
        public OpenImportDocumentViewMessage(CollectionNodeViewModel node, Connection connection, DocumentCollection collection) 
            : base(node, connection, collection)
        {
        }
    }
}
