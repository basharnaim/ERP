using AutoMapper;
using Library.Model.Core.Organizations;
using Library.Service.Core.Organizations;
using Library.ViewModel.Core.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP.WebUI.App_Start
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration ConfigureMapping()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Company, CompanyViewModel>();
                cfg.CreateMap<CompanyViewModel, Company>();
                cfg.CreateMap<Branch, BranchViewModel>();
            });
        }
    }
}