using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Expense
{
    public class Expense
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonElement("category")]
        public string Category { get; set; }
        [BsonElement("price")]
        public double Price { get; set; }
        [BsonElement("title")]
        public string Title { get; set; }
    }
}
