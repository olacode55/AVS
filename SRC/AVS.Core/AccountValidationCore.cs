using AVS.Common;
using AVS.Common.Utiities;
using AVS.Data.Model;
using AVS.Data.ViewModel;
using AVS.Repository;
using AVS.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Core
{
    public class AccountValidationCore
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IClientRepository _clientRepository;
        private readonly IAVSRepository _avsRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly ILoggerManager _loggerManager;
        private readonly IConfiguration _config;
        private readonly AccountValidationResponses _accountResponses;
        private readonly InputValidationCore _inputValidation;
        private readonly FactoryImplemetation _factoryImplemetation;

        public AccountValidationCore(IMemoryCache memoryCache, IProviderRepository providerRepository, IConfiguration config,
                                        IClientRepository clientRepository, IAVSRepository avsRepository, ILoggerManager loggerManager,
                                        AccountValidationResponses accountResponses , InputValidationCore inputValidation,
                                        FactoryImplemetation factoryImplemetation)
        {
            _config = config;
            this._memoryCache = memoryCache;
            _clientRepository = clientRepository;
            _avsRepository = avsRepository;
            _providerRepository = providerRepository;
            _accountResponses = accountResponses;
            _inputValidation = inputValidation;
            _factoryImplemetation = factoryImplemetation;
            _loggerManager = loggerManager;

        }
        public GenericResponse ProcessAccountValidation(AccountValidationVM accountValidationVM , string appId , string ipAddress)
        {
            bool enableDefault = false;
            var resp = new GenericResponse();
            var responseDetails = new AVSDetails();
            var clientList = new List<Client>();
            try
            {
                clientList = CacheUtility.GetAppDetailsCache(_memoryCache, "ClientListCache");
                if (clientList == null)
                {
                    clientList = _clientRepository.GetAllClients();
                    CacheUtility.SetAppDetailsCache(_memoryCache, clientList, "ClientListCache");
                }

                var providerList = new List<Provider>();
                providerList = CacheUtility.GetProviderCache(_memoryCache, "ProviderListCache");
                if (providerList == null)
                {
                    providerList = _providerRepository.GetAllProviders();
                    CacheUtility.SetProviderCache(_memoryCache, providerList, "ProviderListCache");
                }
                string appDefaultProvider = string.Empty;
                var validationResp = _inputValidation.ValidateAccountDetails(accountValidationVM, clientList, ipAddress, appId , out appDefaultProvider);

                if (validationResp != null && validationResp.ResponseCode != null)
                    return validationResp;

                var accountDetails = _avsRepository.FetchAccountDetailsByAccountNumber(accountValidationVM);

                if (accountDetails == null)
                {
                    var provider = providerList.FirstOrDefault(x => x.BankCode == accountValidationVM.BankCode);

                    if(provider == null)
                    {
                        string defaultProvider = string.IsNullOrWhiteSpace(appDefaultProvider) ? _config.GetValue<string>("DefaultProvider") : appDefaultProvider;
                        provider = providerList.FirstOrDefault(x => x.BankCode == defaultProvider);
                        enableDefault = true;

                        if (provider == null)
                            throw new ApplicationException($"Default provider with bank code {defaultProvider} not found");
                    }
                    IFactory processor = _factoryImplemetation.FactoryConsumer(provider.Providername);
                    var processorResponse = enableDefault ? processor.InterBankNameValidation(accountValidationVM, provider) :
                               processor.IntraBankNameValidation(accountValidationVM, provider);

                    //LogRespones
                    if(processorResponse.ResponseCode == "00")
                    {
                        var accountName = new AccountName
                        {
                            AccountNumber = accountValidationVM.AccountNumber,
                            BankCode = accountValidationVM.BankCode,
                            DataCreated = DateTime.Now,
                            CreatedBy = "System",
                            Name = processorResponse.AccountName
                        };

                        _avsRepository.InsertAccountDetails(accountName);
                    }


                    resp = _accountResponses._respDictionary[processorResponse.ResponseCode];
                    responseDetails.Accountname = processorResponse.AccountName;
                    responseDetails.AccountNumber = accountValidationVM.AccountNumber;
                    resp.Data = responseDetails;
                }
                else
                {
                    resp = _accountResponses._respDictionary["00"];
                    responseDetails.Accountname = accountDetails.Name;
                    responseDetails.AccountNumber = accountDetails.AccountNumber;
                    resp.Data = responseDetails;
                }

            }catch(Exception ex)
            {
                _loggerManager.LogError(new {IPaddress = ipAddress ,AccountNumber = accountValidationVM.AccountNumber , accountValidationVM .BankCode , ApplicationId = appId}, ex);
                resp = _accountResponses._respDictionary["10"];
                resp.ResponseMessage = resp.ResponseMessage + ex.Message.Substring(0 , 100); 
            }
            return resp;
        }
    
    }
}
