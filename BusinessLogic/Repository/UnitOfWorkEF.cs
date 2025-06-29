using AutoMapper;
using BusinessLogic.Repository.IRepository;
using DataAccess.Data;
using DataAccess.DbAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class UnitOfWorkEF : IUnitOfWorkEF
    {
        private readonly ApplicationDbContext _db;
        //private readonly IMapper _mapper;
        public UnitOfWorkEF(ApplicationDbContext db) 
        {
            _db = db;
            //Banks = new BankRepository(_db);
        }
        //public IBankRepository Banks { get; private set; }
        public void Save()
        {
            _db.SaveChanges();
        }
    }  
}
