using System;
using Microsoft.EntityFrameworkCore;

namespace IOBootstrap.NET.DataAccess.Context
{
    public class IODatabaseContextDefaultImpl : IODatabaseContext<IODatabaseContextDefaultImpl>
    {
        public IODatabaseContextDefaultImpl(DbContextOptions<IODatabaseContextDefaultImpl> options) : base(options)
        {
        }
    }
}
