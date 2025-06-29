using AppModels;
using AppModels.XcelHsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository.IRepository
{
    public interface IXcelHspSiteRepository
    {
        Task<IEnumerable<ServiceProviders>> GetProviderList(int SearchLimit, string SearchValue);
        Task<IEnumerable<InsuranceCompanies>> GetInsurerList(int SearchLimit, string SearchValue);
        Task<IEnumerable<UserAccountType>> GetUserAccountTypes();
        Task<IEnumerable<ServiceProviders>> GetProviderListByIds(string InsurerId, string ProviderIds);
        Task<IEnumerable<InsuranceCompanies>> GetInsurerListByIds(string InsurerIds);
        Task<string> GetInsurerListByProviderIds(string ProviderIds);
        Task<int> CreateInsurer(CreateInsurerRequest insurer);
        Task<int> UpdateInsurer(UpdateInsurerRequest insurer);
        Task<int> CreateProvider(CreateProviderRequest provider);
        Task<int> UpdateProvider(UpdateProviderRequest provider);
        Task<IEnumerable<ServiceProviders>> GetUnaccreditedProviders(string InsurerId);
        Task<int> AccreditProvider(AccreditProviderRequest accredit);
    }
}
