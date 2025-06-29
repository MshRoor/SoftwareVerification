using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository.IRepository
{
    public interface IUnitOfWorkEF
    {
        //IBankRepository Banks { get; }
        void Save();
    }
}
