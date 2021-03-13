using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expense.Authentication
{
    [CollectionName("Users")]
    public class AppUser : MongoIdentityUser<ObjectId>
    {
        public AppUser() : base()
        {
        }

        public AppUser(string userName, string email) : base(userName, email)
        {
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long? FacebookId { get; set; }
    }
}
