using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expense.Services
{
	public interface IExpensesDbService
	{

		public IEnumerable<Account> GetAccounts();
		public Account GetAccountByName(string name);

	}
}
