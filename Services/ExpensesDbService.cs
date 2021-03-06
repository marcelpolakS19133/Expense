using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Expense.Services
{
	public class ExpensesDbService : IExpensesDbService
	{

		private readonly IMongoDatabase db;

		public ExpensesDbService(IMongoDatabase database)
		{
			db = database;
		}

		public Account GetAccountByName(string name)
		{
			return db.GetCollection<Account>("Konta").Find(a => a.Name.Equals(name)).FirstOrDefault();
		}

		public IEnumerable<Account> GetAccounts()
		{
			var collection = db.GetCollection<Account>("Konta");

			return collection.AsQueryable();
		}
	}
}
