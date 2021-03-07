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
            expense.Id = ObjectId.GenerateNewId();

            var accId = new ObjectId(accountId);
            var update = Builders<Account>.Update.Push("expenses", expense);
            
            _accounts.FindOneAndUpdate(acc => acc.Id.Equals(accId), update);

            return expense;
        }

        public Expense ModifyExpense(string accountId, Expense expense)
        {
            var accId = new ObjectId(accountId);
            var filter = Builders<Account>.Filter.Where(acc => acc.Id.Equals(accId) && acc.Expenses.Any(exp=>exp.Id.Equals(expense.Id)));

            var updates = new List<UpdateDefinition<Account>>();

            if (expense.Price != 0) updates.Add(Builders<Account>.Update.Set("expenses.$.price", expense.Price));
            if (expense.Title != null) updates.Add(Builders<Account>.Update.Set("expenses.$.title", expense.Title));
            if (expense.Category != null) updates.Add(Builders<Account>.Update.Set("expenses.$.category", expense.Category));

            var update = Builders<Account>.Update.Combine(updates);

            var account = _accounts.FindOneAndUpdate(filter, update);

            return expense;
        }

        public Expense DeleteExpense(string accountId, string expenseId)
        {
            var accId = new ObjectId(accountId);
            var expId = new ObjectId(expenseId);
            var filter = Builders<Account>.Filter.Where(acc => acc.Id.Equals(accId));

            var update = Builders<Account>.Update.PullFilter(acc => acc.Expenses, exp => exp.Id.Equals(expId));

            var account = _accounts.FindOneAndUpdate(filter, update);

            return null;
        }
        #endregion
    }
}
