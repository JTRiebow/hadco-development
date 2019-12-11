using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Principal;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;
using Hadco.Common;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Http.OData;
using CsvHelper;
using Hadco.Common.DataTransferObjects;

namespace Hadco.Services
{
    public class EquipmentTimerService : GenericService<EquipmentTimerDto, EquipmentTimer>, IEquipmentTimerService
    {
        private IEquipmentTimerRepository _equipmentTimerRepository;
        private IEquipmentTimerEntryRepository _equipmentTimerEntryRepository;
        private IJobTimerRepository _jobTimerRepository;
        public EquipmentTimerService(IEquipmentTimerRepository equipmentTimerRepository,
            IEquipmentTimerEntryRepository equipmentTimerEntryRepository,
            IJobTimerRepository jobTimerRepository,
            IPrincipal currentUser)
            : base(equipmentTimerRepository, currentUser)
        {
            _equipmentTimerRepository = equipmentTimerRepository;
            _equipmentTimerEntryRepository = equipmentTimerEntryRepository;
            _jobTimerRepository = jobTimerRepository;
        }

        public override EquipmentTimerDto Insert(EquipmentTimerDto dto)
        {
            dto.Diary = RemoveBeginningNull(dto.Diary);

            if (dto.EquipmentServiceTypeID.HasValue)
            {
                dto.EquipmentTimerEntries = new Collection<EquipmentTimerEntryDto>()
                    {
                       Mapper.Map<EquipmentTimerEntryDto>(dto)
                    };
            }

            var equipmentTimer = base.Insert(dto);
            _jobTimerRepository.AddMechanicDowntime(equipmentTimer.TimesheetID);

            return equipmentTimer;
        }

        public override EquipmentTimerDto Update(EquipmentTimerDto dto)
        {
            dto.Diary = RemoveBeginningNull(dto.Diary);

            // Update Timer itself
            var timer = dto;
            timer.EquipmentTimerEntries = null;
            base.Update(timer);

            // Update Equipment Timer Entries
            if (dto.EquipmentServiceTypeID.HasValue || dto.StartTime.HasValue || dto.StopTime.HasValue || dto.Diary != null || dto.Closed.HasValue)
            {
                var equipmentTimerEntries = _equipmentTimerRepository.Find(dto.ID, x => x.EquipmentTimerEntries).EquipmentTimerEntries;
                if (!equipmentTimerEntries.Any())
                {
                    _equipmentTimerEntryRepository.Insert(Mapper.Map<EquipmentTimerEntry>(dto));
                }
                else
                {
                    var equipmentTimerEntry = Mapper.Map<EquipmentTimerEntry>(dto);
                    equipmentTimerEntry.ID = equipmentTimerEntries.First().ID;
                    _equipmentTimerEntryRepository.Update(equipmentTimerEntry);
                }
            }

            _jobTimerRepository.AddMechanicDowntime(dto.TimesheetID);
            return Mapper.Map<EquipmentTimerDto>(_equipmentTimerRepository.FindNoTracking(dto.ID));
        }

        public override void Delete(int id)
        {
            var timesheetID = _equipmentTimerRepository.Find(id).TimesheetID;
            base.Delete(id);
            _jobTimerRepository.AddMechanicDowntime(timesheetID);

        }     

        private static string RemoveBeginningNull(string diary)
        {
            // HACK Phone sending "null" at the beginning of each note. Remove this once fixed
            if (diary != null)
            {
                diary = Regex.Replace(diary, @"^null\n\n\n\n", "");
            }
            return diary;
        }
    }


    public interface IEquipmentTimerService : IGenericService<EquipmentTimerDto>
    {

    }
}
