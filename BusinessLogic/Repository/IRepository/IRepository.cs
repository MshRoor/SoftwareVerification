using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> Find(string Id);
        Task<int> Add(T model);
        Task<int> Update(T model);
        Task<int> Remove(string Id);
    }
}
