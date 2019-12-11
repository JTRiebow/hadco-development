using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;

namespace Hadco.Services
{
    public class EquipmentTimerEntryService : GenericService<EquipmentTimerEntryDto, EquipmentTimerEntry>, IEquipmentTimerEntryService
    {
        private IEquipmentTimerEntryRepository _equipmentTimerEntryRepository;
        public EquipmentTimerEntryService(IEquipmentTimerEntryRepository equipmentTimerEntryRepository, IPrincipal currentUser)
            : base(equipmentTimerEntryRepository, currentUser)
        {
            _equipmentTimerEntryRepository = equipmentTimerEntryRepository;
        }

    }

    public interface IEquipmentTimerEntryService : IGenericService<EquipmentTimerEntryDto>
    {
        
    }
}
