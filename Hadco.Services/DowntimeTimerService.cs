using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Security.Principal;
using AutoMapper;
using CsvHelper;
using Hadco.Common;
using Hadco.Common.DataTransferObjects;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;

namespace Hadco.Services
{
    public class DowntimeTimerService : GenericService<DowntimeTimerDto, DowntimeTimer>, IDowntimeTimerService
    {
        private IDowntimeTimerRepository _downtimeTimerRepository;
        private ILoadTimerRepository _loadTimerRepository;
        public DowntimeTimerService(IDowntimeTimerRepository downtimeTimerRepository, ILoadTimerRepository loadTimerRepository, IPrincipal currentUser)
            : base(downtimeTimerRepository, currentUser)
        {
            _downtimeTimerRepository = downtimeTimerRepository;
            _loadTimerRepository = loadTimerRepository;
        }

        public override DowntimeTimerDto Insert(DowntimeTimerDto dto)
        {       
            // HACK The mobile app does not currently send timesheet ID, so we can get it from the LoadTimer
            if (!dto.TimesheetID.HasValue && dto.LoadTimerID.HasValue)
            {
                dto.TimesheetID = _loadTimerRepository.Find(dto.LoadTimerID.Value)?.TimesheetID;
            }
            var result = base.Insert(dto);
            if (dto.TimesheetID.HasValue)
            {
                _downtimeTimerRepository.AddLoadDowntimeTimers(dto.TimesheetID.Value);
            }
            return result;
        }

        public override DowntimeTimerDto Update(DowntimeTimerDto dto)
        {
            if (!dto.TimesheetID.HasValue && dto.LoadTimerID.HasValue)
            {
                dto.TimesheetID = _loadTimerRepository.Find(dto.LoadTimerID.Value)?.TimesheetID;
            }
            var result = base.Update(dto);
            if (dto.TimesheetID.HasValue)
            {
                _downtimeTimerRepository.AddLoadDowntimeTimers(dto.TimesheetID.Value);
            }
            return result;
        }

        public override void Delete(int id)
        {
            var timesheetID = Find(id)?.TimesheetID;
            base.Delete(id);            
            if (timesheetID.HasValue)
            {
                _downtimeTimerRepository.AddLoadDowntimeTimers(timesheetID.Value);
            }        
        }


        public TruckerDailyDto GetDowntimeTruckerDaily(int downtimeTimerID)
        {
            return _downtimeTimerRepository.GetDowntimeTruckerDaily(downtimeTimerID);
        }
    }

    public interface IDowntimeTimerService : IGenericService<DowntimeTimerDto>
    {
        TruckerDailyDto GetDowntimeTruckerDaily(int downtimeTimerID);
    }
}
