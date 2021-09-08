using AVS.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Repository
{
    public interface IProviderRepository
    {
        List<Provider> GetAllProviders();
        Provider UpdateProvider(Provider provider);
    }
}
