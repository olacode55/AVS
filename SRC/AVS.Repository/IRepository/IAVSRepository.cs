using AVS.Data.Model;
using AVS.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Repository
{
    public interface IAVSRepository
    {
        AccountName FetchAccountDetailsByAccountNumber(AccountValidationVM accountValidationVM);

        void InsertAccountDetails(AccountName details);
    }
}
