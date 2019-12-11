using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Principal;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;
using System.IO;
using Hadco.Common;
using Hadco.Common.DataTransferObjects;

namespace Hadco.Services
{
    public class OccurrenceService : GenericService<OccurrenceDto, Occurrence>, IOccurrenceService
    {
        private IOccurrenceRepository _occurrenceRepository;
        public OccurrenceService(IOccurrenceRepository occurrenceRepository, IPrincipal currentUser)
            : base(occurrenceRepository, currentUser)
        {
            _occurrenceRepository = occurrenceRepository;
        }
    }

    public interface IOccurrenceService : IGenericService<OccurrenceDto>
    {
    }
}
