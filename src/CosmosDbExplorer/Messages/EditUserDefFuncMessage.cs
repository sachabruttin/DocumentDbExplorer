﻿using CosmosDbExplorer.Infrastructure.Models;
using CosmosDbExplorer.ViewModels;
using Microsoft.Azure.Documents;

namespace CosmosDbExplorer.Messages
{
    public class EditUserDefFuncMessage : OpenTabMessageBase<UserDefFuncNodeViewModel>
    {
        public EditUserDefFuncMessage(UserDefFuncNodeViewModel node, Connection connection, DocumentCollection collection) 
            : base(node, connection, collection)
        {
        }
    }
}
