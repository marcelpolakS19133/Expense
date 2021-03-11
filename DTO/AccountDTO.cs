using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expense.DTO
{
    public class AccountDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<ExpenseDTO> Expenses { get; set; }
    }
}
