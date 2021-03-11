using Expense.DTO;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expense.DTOExtensions
{
    public static class ExpenseDTOMapper
    {
        public static Expense ToExpense(this ExpenseDTO dto)
        {
            return new Expense
            {
                Id = new ObjectId(dto.Id),
                Category = dto.Category,
                Price = dto.Price,
                Title = dto.Title
            };
        }
        public static ExpenseDTO ToExpenseDTO(this Expense expense)
        {
            return new ExpenseDTO
            {
                Id = expense.Id.ToString(),
                Category = expense.Category,
                Price = expense.Price,
                Title = expense.Title
            }; 
        }
    }
}
