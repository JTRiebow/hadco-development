﻿using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Hadco.Data
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {               

    }

    public interface IDepartmentRepository : IGenericRepository<Department>
    {

    }
}
