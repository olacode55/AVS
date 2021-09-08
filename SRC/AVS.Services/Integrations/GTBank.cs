﻿using AutoMapper;
using AVS.Common.Utiities;
using AVS.Data.Model;
using AVS.Data.ViewModel;
using AVS.Repository;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Services.Integrations
{
    public class GTBank : IFactory
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly IProviderRepository _providerRepository;
        private readonly IMemoryCache _memoryCache;
        public GTBank(IMapper mapper ,IConfiguration config, IMemoryCache memoryCache, IProviderRepository providerRepository)
        {
            _mapper = mapper;
            _config = config;
            _memoryCache = memoryCache;
            _providerRepository = providerRepository;
        }
        public GTAuthResponseVM Authenticate(string baseUrl , string method)
        {
            var gtBankConfig  = new GTBankConfig();
            _config.GetSection("GTBankConfig").Bind(gtBankConfig);
            var client = new RestClient(baseUrl);
            RestRequest request = new RestRequest(method, Method.POST);
            request.AddJsonBody(JsonConvert.SerializeObject(gtBankConfig));
            var response = client.Execute(request);

            if (!response.IsSuccessful || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.ErrorMessage == null)
                    throw new ApplicationException(response.Content.Trim());
                else
                    throw new ApplicationException(response.ErrorMessage);
            }

            var resp = JsonConvert.DeserializeObject<GTAuthResponseVM>(response.Content);
            return resp;
        }
        public GenericProviderResponse InterBankNameValidation(AccountValidationVM accountNameVM, Provider provider)
        {
            string token = provider.AccessToken;
            if (string.IsNullOrEmpty(token) || provider.TokenExpiryDate < DateTime.Now)
            {
                var auth = Authenticate(provider.BaseURL, provider.AuthMethod);
                provider.AccessToken = auth.Data.BearerToken;
                provider.TokenExpiryDate = auth.Data.ExpiryTime;

                _providerRepository.UpdateProvider(provider);

                CacheUtility.RemoveCache(_memoryCache , "ProviderListCache");
            }

            var client = new RestClient(provider.BaseURL);
            RestRequest request = new RestRequest(provider.InterBankMethod, Method.GET);
            request.AddHeader("Authorization", string.Format("Bearer {0}", provider.AccessToken));
            request.AddParameter("bankCode", accountNameVM.BankCode, ParameterType.QueryString);
            request.AddParameter("accountNumber", accountNameVM.AccountNumber, ParameterType.QueryString);
            var response = client.Execute(request);

            if (!response.IsSuccessful || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if(response.ErrorMessage == null)
                    throw new ApplicationException(response.Content.Trim());
                else
                    throw new ApplicationException(response.ErrorMessage);
            }

            var resp = JsonConvert.DeserializeObject<GTNameValidationResponse>(response.Content);
            resp.Data.ResponseCode = resp.message.ToLower() == "success" ? "00" : "05";
            //return resp;
            var respMap = _mapper.Map<GenericProviderResponse>(resp.Data);
            return respMap;
        }

        public GenericProviderResponse IntraBankNameValidation(AccountValidationVM accountNameVM, Provider provider)
        {
            string token = provider.AccessToken;
            if (string.IsNullOrEmpty(token) || provider.TokenExpiryDate < DateTime.Now)
            {
                var auth = Authenticate(provider.BaseURL, provider.AuthMethod);
                provider.AccessToken = auth.Data.BearerToken;
                provider.TokenExpiryDate = auth.Data.ExpiryTime;

                _providerRepository.UpdateProvider(provider);
                CacheUtility.RemoveCache(_memoryCache, "ProviderListCache");
            }

            var client = new RestClient(provider.BaseURL +'/'+provider.IntraBankMethod);
            RestRequest request = new RestRequest(accountNameVM.AccountNumber, Method.GET);
            request.AddHeader("Authorization", string.Format("Bearer {0}", provider.AccessToken));
            //request.AddParameter("Nuban", accountNameVM.AccountNumber, ParameterType.QueryString);
            var response = client.Execute(request);

            if (!response.IsSuccessful || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                if (response.ErrorMessage == null)
                    throw new ApplicationException(response.Content.Trim());
                else
                    throw new ApplicationException(response.ErrorMessage);
            }

            var resp = JsonConvert.DeserializeObject<GTNameValidationResponse>(response.Content);
            resp.Data.ResponseCode = resp.message.ToLower() == "success" ? "00" : "05"; 
            var respMap = _mapper.Map<GenericProviderResponse>(resp.Data);
            return respMap;
        }
    }
}
