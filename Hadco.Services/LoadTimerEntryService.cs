using System.Linq;
using System.Security.Principal;
using Hadco.Common.Exceptions;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;

namespace Hadco.Services
{
    public class LoadTimerEntryService : GenericService<LoadTimerEntryDto, LoadTimerEntry>, ILoadTimerEntryService
    {
        private ILoadTimerEntryRepository _loadTimerEntryRepository;
        private ILoadTimerRepository _loadTimerRepository;
        private IDowntimeTimerRepository _downtimeTimerRepository;
        public LoadTimerEntryService(ILoadTimerEntryRepository loadTimerEntryRepository,
                                     ILoadTimerRepository loadTimerRepository,
                                     IDowntimeTimerRepository downtimeTimerRepository,
                                     IPrincipal currentUser)
            : base(loadTimerEntryRepository, currentUser)
        {
            _loadTimerEntryRepository = loadTimerEntryRepository;
            _loadTimerRepository = loadTimerRepository;
            _downtimeTimerRepository = downtimeTimerRepository;
        }

        public override LoadTimerEntryDto Insert(LoadTimerEntryDto dto)
        {
            CheckForTimerOverlap(dto);
            var loadTimerEntryDto = base.Insert(dto);
            _downtimeTimerRepository.AddLoadDowntimeTimers(_loadTimerEntryRepository.GetTimesheetID(dto.ID));
            return loadTimerEntryDto;
        }

        public override LoadTimerEntryDto Update(LoadTimerEntryDto dto)
        {
            var loadTimerEntryDto = base.Update(dto);
            _downtimeTimerRepository.AddLoadDowntimeTimers(_loadTimerEntryRepository.GetTimesheetID(dto.ID));
            return loadTimerEntryDto;
        }

        public override void Delete(int id)
        {
            base.Delete(id);
            _downtimeTimerRepository.AddLoadDowntimeTimers(_loadTimerEntryRepository.GetTimesheetID(id));
        }

        private void CheckForTimerOverlap(LoadTimerEntryDto dto)
        {
            var timesheetID = _loadTimerEntryRepository.GetTimesheetID(dto.ID);
            CheckForTimerOverlap(dto, timesheetID);
        }

        public void CheckForTimerOverlap(LoadTimerEntryDto dto, int timesheetID)
        {
            var timers = _loadTimerEntryRepository.GetAllLoadTimerEntriesOnTimesheet(timesheetID);

            var overlappingTimers = timers.Where(x => (x.StartTime < dto.StartTime && dto.StartTime < x.EndTime) ||
                                                      (x.StartTime < dto.EndTime && dto.EndTime < x.EndTime) ||
                                                      (dto.StartTime < x.StartTime && x.StartTime < dto.EndTime) ||
                                                      (dto.StartTime < x.EndTime && x.EndTime < dto.EndTime));

            if (overlappingTimers.Any())
            {
                throw new TimerOverlapException(
                    "Cannot update load timer due to overlap with an existing load timer for this employee");
            }
        }
    }

    public interface ILoadTimerEntryService : IGenericService<LoadTimerEntryDto>
    {
        void CheckForTimerOverlap(LoadTimerEntryDto dto, int timesheetID);
    }
}