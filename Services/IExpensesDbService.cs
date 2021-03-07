using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expense.Services
{
	public interface IExpensesDbService
	{
        #region accounts
        public IEnumerable<Account> GetAccounts(bool withExpenses);
		public Account GetAccountById(string id, bool withExpenses);
		public Account CreateAccount(Account account);
		public Account ModifyAccount(Account account);
		public Account DeleteAccount(string id);
        #endregion

        #region expenses
        public Expense AddExpense(string accountId, Expense expense);
        public Expense ModifyExpense(string accountId, Expense expense);
        public Expense DeleteExpense(string accountId, string expenseId);
        #endregion
    }
}
