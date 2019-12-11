using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Hadco.Services
{
    public class NoteTypeService : GenericService<NoteTypeDto, NoteType>, INoteTypeService
    {
        INoteTypeRepository _noteTypeRepository; 
        public NoteTypeService(INoteTypeRepository noteTypeRepository, IPrincipal currentUser) : base(noteTypeRepository, currentUser)
        {
            _noteTypeRepository = noteTypeRepository;
        }

        public IEnumerable<NoteTypeDto> GetFromCache()
        {
                var dtos = MemoryCache.Default.Get(typeof(NoteTypeDto).Name) as IEnumerable<NoteTypeDto>;
                if (dtos == null)
                {
                    dtos = Mapper.Map<IEnumerable<NoteTypeDto>>(_noteTypeRepository.All);
                    MemoryCache.Default.Set(typeof(NoteTypeDto).Name, dtos, DateTimeOffset.MaxValue);
                }
                return dtos;
            }
    }

    public interface INoteTypeService : IGenericService<NoteTypeDto>
    {
        IEnumerable<NoteTypeDto> GetFromCache();
    }
}
