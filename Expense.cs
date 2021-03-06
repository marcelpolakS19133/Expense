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
        [BsonElement("Category")]
        public String Category { get; set; }
        [BsonElement("Price")]
        public Double Price { get; set; }
        [BsonElement("Title")]
        public String Title { get; set; }
    }
}
