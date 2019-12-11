using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Principal;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;

namespace Hadco.Services
{
	public class BillTypeService : GenericService<BillTypeDto, BillType>, IBillTypeService
    {
	    private IBillTypeRepository _billTypeRepository;
        public BillTypeService(IBillTypeRepository billTypeRepository, IPrincipal currentUser)
            : base(billTypeRepository, currentUser)
		{
			_billTypeRepository = billTypeRepository;
		}
        public IEnumerable<BillTypeDto> GetFromCache()
        {
            var dtos = MemoryCache.Default.Get(typeof(BillTypeDto).Name) as IEnumerable<BillTypeDto>;
            if (dtos == null)
            {
                dtos = Mapper.Map<IEnumerable<BillTypeDto>>(_billTypeRepository.All);
                MemoryCache.Default.Set(typeof(BillTypeDto).Name, dtos, DateTimeOffset.MaxValue);
            }
            return dtos;
        }
    }

	public interface IBillTypeService : IGenericService<BillTypeDto>
	{
	    IEnumerable<BillTypeDto> GetFromCache();

	}
}
