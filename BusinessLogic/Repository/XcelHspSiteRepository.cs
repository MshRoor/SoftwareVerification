using AppModels.XcelHsp;
using BusinessLogic.Repository.IRepository;
using Dapper;
using DataAccess.DbAccess;
using System.Data;
using System.Security.Claims;

namespace BusinessLogic.Repository
{
    public class XcelHspSiteRepository : IXcelHspSiteRepository
    {
        private readonly ISqlDataAccess _db;
        public XcelHspSiteRepository(ISqlDataAccess db)
        {
            _db = db;
        }
        public async Task<IEnumerable<ServiceProviders>> GetProviderList(int SearchLimit, string SearchValue)
        {
            string query = @"SELECT top(@SearchLimit) ID, ServiceProvider, InsurerIds, CreatedDate FROM ServiceProviders WHERE (ServiceProvider LIKE '%' + @SearchValue + '%') ORDER BY ServiceProvider";
            using IDbConnection connection = _db.CreateXcelHspConnection();
            return await connection.QueryAsync<ServiceProviders>(query, new { SearchLimit = SearchLimit, SearchValue = SearchValue });
        }
        public async Task<IEnumerable<InsuranceCompanies>> GetInsurerList(int SearchLimit, string SearchValue)
        {
            string query = @"SELECT top(@SearchLimit) ID, CompanyName, CreatedDate FROM InsuranceCompanies WHERE (CompanyName LIKE '%' + @SearchValue + '%') ORDER BY CompanyName";
            using IDbConnection connection = _db.CreateXcelHspConnection();
            return await connection.QueryAsync<InsuranceCompanies>(query, new { SearchLimit = SearchLimit, SearchValue = SearchValue });
        }
        public async Task<IEnumerable<UserAccountType>> GetUserAccountTypes()
        {
            string query = @"SELECT AccountType FROM UserAccountType";
            using IDbConnection connection = _db.CreateXcelHspConnection();
            return await connection.QueryAsync<UserAccountType>(query, new { });
        }
        public async Task<IEnumerable<ServiceProviders>> GetProviderListByIds(string InsurerId, string ProviderIds)
        {
            string query = "";
            if (InsurerId != "0")
                query = @"SELECT ID,ServiceProvider,InsurerIds,CreatedDate,GPSAddress,ImageUrl FROM ServiceProviders WHERE ',' + InsurerIds + ',' LIKE '%,' + @InsurerId + ',%' 
                                        AND ID IN (SELECT * FROM dbo.IDsToTable(@ProviderIds)) ORDER BY ServiceProvider";
            else
                query = @"SELECT ID,ServiceProvider,InsurerIds,CreatedDate,GPSAddress,ImageUrl FROM ServiceProviders WHERE ID IN (SELECT * FROM dbo.IDsToTable(@ProviderIds)) ORDER BY ServiceProvider";
            using IDbConnection connection = _db.CreateXcelHspConnection();
            return await connection.QueryAsync<ServiceProviders>(query, new { InsurerId = InsurerId, ProviderIds = ProviderIds });
        }
        public async Task<IEnumerable<InsuranceCompanies>> GetInsurerListByIds(string InsurerIds)
        {
            string query = @"SELECT ID, CompanyName, UIColor, APIUrl, LogoUrl FROM InsuranceCompanies WHERE ID IN (SELECT * FROM dbo.IDsToTable(@InsurerIds)) ORDER BY CompanyName";
            using IDbConnection connection = _db.CreateXcelHspConnection();
            return await connection.QueryAsync<InsuranceCompanies>(query, new { InsurerIds = InsurerIds });
        }
        public async Task<string> GetInsurerListByProviderIds(string ProviderIds)
        {
            string query = @"SELECT InsurerIds FROM ServiceProviders WHERE ID IN (SELECT * FROM dbo.IDsToTable(@ProviderIds))";
            using IDbConnection connection = _db.CreateXcelHspConnection();
            List<string> InsurerIdList = (await connection.QueryAsync<string>(query, new { ProviderIds = ProviderIds })).ToList();
            HashSet<string> uniqueInsurerIds = new HashSet<string>();

            // Iterate through each item in the list
            foreach (var item in InsurerIdList)
            {
                var insurerIds = item.Split(',');
                                   //.Select(int.Parse); // Convert each substring to an integer
                foreach (var num in insurerIds)
                {
                    uniqueInsurerIds.Add(num);
                }
            }
            string result = uniqueInsurerIds.Count != 0 ? string.Join(",", uniqueInsurerIds) : "0";
            return result;
        }
        public async Task<int> CreateInsurer(CreateInsurerRequest insurer)
        {
            return await _db.SaveDataAsync(query: "Insert Into InsuranceCompanies(CompanyName,UIColor,APIUrl,LogoUrl) Values(@CompanyName,@UIColor,@APIUrl,@LogoUrl)", insurer );
        }
        public async Task<int> UpdateInsurer(UpdateInsurerRequest insurer)
        {
            return await _db.SaveDataAsync(query: "Update InsuranceCompanies Set CompanyName=@CompanyName,UIColor=@UIColor,APIUrl=@APIUrl,LogoUrl=@LogoUrl Where Id=@Id", insurer );
        }
        public async Task<int> CreateProvider(CreateProviderRequest provider)
        {
            return await _db.SaveDataAsync(query: "Insert Into ServiceProviders(ServiceProvider,InsurerIds,GPSAddress,ImageUrl) Values(@ServiceProvider,@InsurerIds,@GPSAddress,@ImageUrl)", provider );
        }
        public async Task<int> UpdateProvider(UpdateProviderRequest provider)
        {
            return await _db.SaveDataAsync(query: "Update ServiceProviders Set ServiceProvider=@ServiceProvider,InsurerIds=@InsurerIds,GPSAddress=@GPSAddress,ImageUrl=@ImageUrl Where Id=@Id", provider);
        }
        public async Task<IEnumerable<ServiceProviders>> GetUnaccreditedProviders(string InsurerId)
        {
            string query = @"SELECT ID,ServiceProvider,GPSAddress FROM ServiceProviders WHERE ',' + InsurerIds + ',' NOT LIKE '%,' + @InsurerId + ',%' ORDER BY ServiceProvider";
            using IDbConnection connection = _db.CreateXcelHspConnection();
            return await connection.QueryAsync<ServiceProviders>(query, new { InsurerId = InsurerId });
        }
        public async Task<int> AccreditProvider(AccreditProviderRequest accredit)
        {
            return await _db.SaveDataAsync(query: "Update ServiceProviders set InsurerIds = InsurerIds + ',' + @InsurerId Where Id=@ProviderId", accredit);
        }
    }
}
