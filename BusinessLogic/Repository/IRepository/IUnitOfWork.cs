using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository.IRepository
{
    public interface IUnitOfWork
    {
        IXcelHspSiteRepository XcelHspSite { get; }
    }
}
