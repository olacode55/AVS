using AVS.Data.Model;
using AVS.Data.ViewModel;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Repository
{
    public class AVSRepository : IAVSRepository
    {
        private readonly IDapper _dapper;

        public AVSRepository(IDapper dapper)
        {
            _dapper = dapper;
        }
        public AccountName FetchAccountDetailsByAccountNumber(AccountValidationVM accountValidationVM)
        {
            var dbPara = new DynamicParameters();
            dbPara.Add("AccountNumber", accountValidationVM.AccountNumber);
            dbPara.Add("BankCode", accountValidationVM.BankCode, DbType.String);

            var accountDetails = _dapper.Get<AccountName>("[dbo].[sp_GetAccountDetails]", dbPara, commandType: CommandType.StoredProcedure);
            return accountDetails;

        }

        public void InsertAccountDetails(AccountName details)
        {
            var date = details.DataCreated.ToString("yyyy-MM-dd HH: mm:ss");
            var dbPara = new DynamicParameters();
            dbPara.Add("Name", details.Name);
            dbPara.Add("AccountNumber", details.AccountNumber);
            dbPara.Add("BankCode", details.BankCode, DbType.String);
            dbPara.Add("CreatedBy", details.CreatedBy);
            dbPara.Add("DataCreated", details.DataCreated.ToString("yyyy-MM-dd HH: mm:ss") , DbType.DateTime);


            var accountDetails = _dapper.Insert<AccountName>("[dbo].[sp_InsertAccountDetail]", dbPara, commandType: CommandType.StoredProcedure);
            //return accountDetails;
        }
    }
}
