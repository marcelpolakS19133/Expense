using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Expense.Services
{
    public interface IAuthService
    {
        public object DoAuth(string code);
    }
}
