using AVS.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Repository
{
    public interface IClientRepository
    {
        List<Client> GetAllClients();
    }
}
