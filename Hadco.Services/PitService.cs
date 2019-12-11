using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Principal;
using Hadco.Common;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;

namespace Hadco.Services
{
    public class PitService : GenericService<PitDto, Pit>, IPitService
    {
        private IPitRepository _pitRepository;

        public PitService(IPitRepository pitRepository, IPrincipal currentUser)
            : base(pitRepository, currentUser)
        {
            _pitRepository = pitRepository;
        }

        public IEnumerable<PitDto> GetFromCache()
        {
            var dtos = MemoryCache.Default.Get(typeof(PitDto).Name) as IEnumerable<PitDto>;
            if (dtos == null)
            {
                dtos = Mapper.Map<IEnumerable<PitDto>>(_pitRepository.All);
                MemoryCache.Default.Set(typeof(PitDto).Name, dtos, DateTimeOffset.MaxValue);
            }
            return dtos;
        }
    }

    public interface IPitService : IGenericService<PitDto>
    {
        IEnumerable<PitDto> GetFromCache();

    }
}
