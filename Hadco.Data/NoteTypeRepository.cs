using Hadco.Common.Enums;
using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Hadco.Data
{
    public class NoteTypeRepository : GenericRepository<NoteType>, INoteTypeRepository
    {

    }

    public interface INoteTypeRepository : IGenericRepository<NoteType>
    {

    }
}
