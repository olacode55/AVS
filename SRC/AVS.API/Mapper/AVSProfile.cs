using AutoMapper;
using AVS.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Com.Xpresspayments.AVS.API.Mapper
{
    public class AVSProfile : Profile
    {
        public AVSProfile()
        {
            CreateMap<FidelityResponseVM, GenericProviderResponse>();
            CreateMap<GTNameEnquiryDetails, GenericProviderResponse>();
        }
        
    }
}
