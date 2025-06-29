using AutoMapper;
using BusinessLogic.Repository.IRepository;
using DataAccess.DbAccess;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ISqlDataAccess _db;
        //private readonly IMapper _mapper;
        public UnitOfWork(ISqlDataAccess db) 
        {
            _db = db;
            //_mapper = mapper;
            XcelHspSite = new XcelHspSiteRepository(_db);
        }
        public IXcelHspSiteRepository XcelHspSite { get; private set; }
    }
}
