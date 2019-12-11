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

namespace Hadco.Services
{
	public class EquipmentServiceTypeService : GenericService<EquipmentServiceTypeDto, EquipmentServiceType>, IEquipmentServiceTypeService
    {
	    private IEquipmentServiceTypeRepository _equipmentServiceTypeRepository;
        public EquipmentServiceTypeService(IEquipmentServiceTypeRepository equipmentServiceTypeRepository, IPrincipal currentUser)
            : base(equipmentServiceTypeRepository, currentUser)
		{
			_equipmentServiceTypeRepository = equipmentServiceTypeRepository;
		}
	}

	public interface IEquipmentServiceTypeService : IGenericService<EquipmentServiceTypeDto>
	{

	}
}
