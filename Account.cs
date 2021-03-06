using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Expense
{
    public class Account
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("name")]
        public String Name { get; set; }
        [BsonElement("expenses")]
        public Object[] Expenses { get; set; }


    }
}
