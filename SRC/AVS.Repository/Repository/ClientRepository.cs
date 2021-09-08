using AVS.Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly IDapper _dapper;
        public ClientRepository(IDapper dapper)
        {
            _dapper = dapper;
        }
        public List<Client> GetAllClients()
        {
            var clientList = _dapper.GetAll<Client>("[dbo].[sp_GetAllClients]", null, commandType: CommandType.StoredProcedure);
            return clientList;
        }
    }
}
