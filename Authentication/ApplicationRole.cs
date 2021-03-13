using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expense.Authentication
{
    [CollectionName("Roles")]
    public class ApplicationRole : MongoIdentityRole<ObjectId>
    {
        public ApplicationRole() : base()
        {

        }
        public ApplicationRole(string rolename) : base(rolename)
        {

        }
    }
}
