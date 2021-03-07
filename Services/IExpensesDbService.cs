using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expense.Services
{
	public interface IExpensesDbService
	{

		public IEnumerable<Account> GetAccounts(bool withExpenses);
		public Account GetAccountByName(string name);

		public IEnumerable<Expense> GetExpensesForAccountName(string name);

		public IEnumerable<Expense> AddExpense(string name, Expense expense); 

	}
}
