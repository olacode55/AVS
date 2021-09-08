using AutoMapper;
using AVS.Data.Model;
using AVS.Data.ViewModel;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Services
{
    public class FidelityBank :IFactory
    {
        private readonly IMapper _mapper;
        public FidelityBank(IMapper mapper)
        {
            _mapper = mapper;
        }
        public GenericProviderResponse InterBankNameValidation(AccountValidationVM accountNameVM , Provider provider)
        {
            var requestModel = new FidelityRequestVM
            {
                DestinationAccount = accountNameVM.AccountNumber,
                DestinationBankCode = accountNameVM.BankCode,
            };
            var client = new RestClient(provider.BaseURL );
            RestRequest request = new RestRequest(provider.InterBankMethod, Method.POST);
            request.AddJsonBody(JsonConvert.SerializeObject(requestModel));
            var response = client.Execute(request);
            
            if (!response.IsSuccessful || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.ErrorMessage == null)
                    throw new ApplicationException(response.Content.Trim());
                else
                    throw new ApplicationException(response.ErrorMessage);
            }

            var resp = JsonConvert.DeserializeObject<FidelityResponseVM>(response.Content);
            resp.ResponseCode = string.IsNullOrWhiteSpace(resp.AccountName) ? "05" : "00";
            var respMap = _mapper.Map<GenericProviderResponse>(resp);
            return respMap;
        }

        public GenericProviderResponse IntraBankNameValidation(AccountValidationVM accountNameVM , Provider provider)
        {
            var client = new RestClient(provider.BaseURL);
            RestRequest request = new RestRequest(provider.IntraBankMethod, Method.GET);
            request.AddParameter("AccountNumber", accountNameVM.AccountNumber, ParameterType.QueryString);
            var respContent = client.Execute(request);

            if (!respContent.IsSuccessful || respContent.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if(respContent.ErrorMessage == null)
                    throw new ApplicationException(respContent.Content.Trim());
                else
                    throw new ApplicationException(respContent.ErrorMessage);
            }

            var resp = JsonConvert.DeserializeObject<FidelityResponseVM>(respContent.Content);
            resp.ResponseCode = string.IsNullOrWhiteSpace(resp.AccountName) ? "05" : "00";

            var respMap = _mapper.Map<GenericProviderResponse>(resp);
            return respMap;
        }

       
    }
}
