using AVS.Data.Model;
using AVS.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Services
{
    public interface IFactory
    {
        GenericProviderResponse InterBankNameValidation(AccountValidationVM accountNameVM, Provider provider);
        GenericProviderResponse IntraBankNameValidation(AccountValidationVM accountNameVM, Provider provider);
    }
}
