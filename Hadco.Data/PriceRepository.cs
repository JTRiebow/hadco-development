using System;
using Hadco.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using Hadco.Common.DataTransferObjects;

namespace Hadco.Data
{
    public class PriceRepository : GenericRepository<Price>, IPriceRepository
    {
    }
    public interface IPriceRepository : IGenericRepository<Price>
    {
    }
}