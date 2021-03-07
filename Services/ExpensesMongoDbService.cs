using System;
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
        #region accounts
        public IEnumerable<Account> GetAccounts(bool withExpenses)
        {
            return withExpenses ? _accounts.AsQueryable() : _accounts.AsQueryable().Select(acc => new Account { Id = acc.Id, Name = acc.Name });
        }

        public Account GetAccountById(string id, bool withExpenses)
        {
            var accId = new ObjectId(id);
            return withExpenses ? _accounts.Find(acc => acc.Id.Equals(accId)).SingleOrDefault()
                                : _accounts.AsQueryable().Select(acc => new Account { Id = acc.Id, Name = acc.Name })
                                                         .Where(acc => acc.Id.Equals(accId))
                                                         .SingleOrDefault();
        }

        public Account CreateAccount(Account account)
        {
            account.Id = ObjectId.GenerateNewId();
            _accounts.InsertOne(account);
            return account;
        }

        public Account ModifyAccount(Account account)
        {
            var updates = new List<UpdateDefinition<Account>>();

            if (account.Name != null) updates.Add(Builders<Account>.Update.Set("name", account.Name));
            if (account.Expenses != null) updates.Add(Builders<Account>.Update.Set("expenses", account.Expenses));

            if (updates.Count == 0) return null;

            var update = Builders<Account>.Update.Combine(updates);

            return _accounts.FindOneAndUpdate(acc => acc.Id.Equals(account.Id), update);
        }

        public Account DeleteAccount(string id)
        {
            var accId = new ObjectId(id);
            return _accounts.FindOneAndDelete(acc => acc.Id.Equals(accId));
        }
        #endregion
        #region expenses
        public Expense AddExpense(string accountId, Expense expense)
        {
            throw new NotImplementedException();
        }

        public Expense ModifyExpense(string accountId, Expense expense)
        {
            throw new NotImplementedException();
        }

        public Expense DeleteExpense(string accountId, string expenseId)
        {
            throw new NotImplementedException();
        }
        #endregion
        //public Account GetAccountByName(string name)
        //{
        //	return _accounts.Find(a => a.Name.Equals(name)).FirstOrDefault();
        //}

        //public IEnumerable<Expense> GetExpensesForAccountName(string name)
        //{
        //	return GetAccountByName(name).Expenses.AsQueryable();
        //}

        //public IEnumerable<Expense> AddExpense(string name, Expense expense)
        //{

        //	var filter = Builders<Account>.Filter.And(
        //		Builders<Account>.Filter.Where(x => x.Name == name)
        //		);

        //	expense.Id = ObjectId.GenerateNewId();

        //	var update = Builders<Account>.Update.Push("expenses", expense);

        //	_accounts.FindOneAndUpdate(filter, update);

        //	return GetExpensesForAccountName(name);
        //}
    }
}
