﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Expense.Services
{
	public class ExpensesMongoDbService : IExpensesDbService
	{

		private readonly IMongoDatabase db;
		private readonly IMongoCollection<Account> _accounts;

		public ExpensesMongoDbService(IMongoDatabase database)
		{
			db = database;
			_accounts = db.GetCollection<Account>("Konta");
		}

		public IEnumerable<Account> GetAccounts(bool withExpenses)
		{
			return withExpenses ? _accounts.AsQueryable() : _accounts.AsQueryable().Select(acc => new Account{ Id = acc.Id, Name = acc.Name });
		}

		public Account GetAccountByName(string name)
		{
			return _accounts.Find(a => a.Name.Equals(name)).FirstOrDefault();
		}

		public IEnumerable<Expense> GetExpensesForAccountName(string name)
		{
			return GetAccountByName(name).Expenses.AsQueryable();
		}

		public IEnumerable<Expense> AddExpense(string name, Expense expense)
		{

			var filter = Builders<Account>.Filter.And(
				Builders<Account>.Filter.Where(x => x.Name == name)
				);

			expense.Id = ObjectId.GenerateNewId();

			var update = Builders<Account>.Update.Push("expenses", expense);

			_accounts.FindOneAndUpdate(filter, update);

			return GetExpensesForAccountName(name);
		}
	}
}