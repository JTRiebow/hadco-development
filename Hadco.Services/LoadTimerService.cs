using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using AutoMapper;
using System.Security.Principal;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;
using CsvHelper;
using Hadco.Common;
using Hadco.Common.DataTransferObjects;
using Hadco.Common.Exceptions;

namespace Hadco.Services
{
    public class LoadTimerService : GenericService<LoadTimerDto, LoadTimer>, ILoadTimerService
    {
        private ILoadTimerRepository _loadTimerRepository;
        private ILoadTimerEntryRepository _loadTimerEntryRepository;
        private IDowntimeTimerRepository _downtimeTimerRepository;
        private ICategoryRepository _categoryRepository;
        private IPhaseRepository _phaseRepository;
        private ITruckClassificationRepository _truckClassificationRepository;
        private ILoadTimerEntryService _loadTimerEntryService;
        private IPricingService _pricingService;

        public LoadTimerService(ILoadTimerRepository loadTimerRepository,
                                ILoadTimerEntryRepository loadTimerEntryRepository,
                                IDowntimeTimerRepository downtimeTimerRepository,
                                ICategoryRepository categoryRepository,
                                IPhaseRepository phaseRepository,
                                ITruckClassificationRepository truckClassificationRepository,
                                ILoadTimerEntryService loadTimerEntryService,
                                IPricingService pricingService,
                                IPrincipal currentUser)
            : base(loadTimerRepository, currentUser)
        {
            _loadTimerRepository = loadTimerRepository;
            _loadTimerEntryRepository = loadTimerEntryRepository;
            _downtimeTimerRepository = downtimeTimerRepository;
            _categoryRepository = categoryRepository;
            _phaseRepository = phaseRepository;
            _truckClassificationRepository = truckClassificationRepository;
            _loadTimerEntryService = loadTimerEntryService;
            _pricingService = pricingService;
        }

        public override void Delete(int id)
        {
            _downtimeTimerRepository.DeleteDowntimeTimers(id);
            base.Delete(id);
        }

        public LoadTimerDto InsertNativeApp(LoadTimerDto dto)
        {
            dto.LoadTimerEntries = new Collection<LoadTimerEntryDto>()
                    {
                        Mapper.Map<LoadTimerEntryDto>(dto)
                    };

            CheckJobPhaseCategoryConsistency(dto);
            return Insert(dto);
        }

        public override LoadTimerDto Insert(LoadTimerDto dto)
        {
            CheckJobPhaseCategoryConsistency(dto);

            var timesheetID = dto.TimesheetID;
            foreach (var entry in dto.LoadTimerEntries ?? new List<LoadTimerEntryDto>())
            {
                _loadTimerEntryService.CheckForTimerOverlap(entry, timesheetID);
            }
            dto.TruckClassificationID = _truckClassificationRepository.GetTruckClassificationID(dto.TruckID, dto.TrailerID, dto.PupID);
            dto.PricePerUnit = _pricingService.FindPrice(dto);

            var loadTimer = base.Insert(dto);

            HostingEnvironment.QueueBackgroundWorkItem(ct => _downtimeTimerRepository.AddLoadDowntimeTimers(timesheetID));
            return loadTimer;
        }

        public override LoadTimerDto Update(LoadTimerDto dto)
        {
            dto.LoadTimerEntries = null;
            CheckJobPhaseCategoryConsistency(dto);
            var loadTimer = _loadTimerRepository.Find(dto.ID);
            Mapper.Map(dto, loadTimer);

            loadTimer.TruckClassificationID = _truckClassificationRepository.GetTruckClassificationID(dto.TruckID, dto.TrailerID, dto.PupID);
            loadTimer.PricePerUnit = _pricingService.FindPrice(dto);

            _loadTimerRepository.Save();

            HostingEnvironment.QueueBackgroundWorkItem(ct => _downtimeTimerRepository.AddLoadDowntimeTimers(dto.TimesheetID));
            return Mapper.Map<LoadTimerDto>(_loadTimerRepository.Find(dto.ID, x => x.LoadTimerEntries));
        }

        public void CheckJobPhaseCategoryConsistency(LoadTimerDto dto)
        {
            if (dto.CategoryID.HasValue)
            {
                var category = _categoryRepository.Find(dto.CategoryID.Value);

                if (dto.PhaseID != category.PhaseID)
                {
                    throw new ArgumentException("Phase does not match Category");
                }
                if (dto.JobID != category.JobID)
                {
                    throw new ArgumentException("Job does not match Category");
                }
            }

            if (dto.PhaseID.HasValue)
            {
                var phase = _phaseRepository.Find(dto.PhaseID.Value);

                if (dto.JobID != phase.JobID)
                {
                    throw new ArgumentException("Job does not match Phase");
                }
            }
        }

        public LoadTimerPrimaryDto FindPrimary(int loadTimerID)
        {
            var loadTimer = _loadTimerRepository
            .AllIncluding(y => y.Job,
                          y => y.Phase,
                          y => y.Category,
                          y => y.DowntimeTimers,
                          y => y.Truck,
                          y => y.Trailer,
                          y => y.Pup,
                          y => y.Material,
                          y => y.Timesheet,
                          y => y.BillType,
                          y => y.LoadTimerEntries).SingleOrDefault(x => x.ID == loadTimerID);

            return Mapper.Map<LoadTimerPrimaryDto>(loadTimer);
        }


        public IEnumerable<LoadTimerExpandedDto> GetAllExpanded(int timesheetID, NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<LoadTimer> query;
            if (filter != null)
            {
                query = _loadTimerRepository.AllIncluding(
                                              y => y.Job,
                                              y => y.Phase,
                                              y => y.Category,
                                              y => y.DowntimeTimers,
                                              y => y.Truck,
                                              y => y.Trailer,
                                              y => y.Pup,
                                              y => y.Material,
                                              y => y.BillType,
                                              y => y.LoadTimerEntries
                                              ).Filter(filter);
            }
            else
            {
                query = _loadTimerRepository.AllIncluding(
                                              y => y.Job,
                                              y => y.Phase,
                                              y => y.Category,
                                              y => y.DowntimeTimers,
                                              y => y.Truck,
                                              y => y.Trailer,
                                              y => y.Pup,
                                              y => y.Material,
                                              y => y.BillType,
                                              y => y.LoadTimerEntries);
            }

            query = query.Where(x => x.TimesheetID == timesheetID);

            // if there is pagination then run the query twice, else convert to a list to get the count so that the query only runs once.
            List<LoadTimer> result;
            if (pagination != null)
            {
                totalResultCount = query.Count();
                result = query.Pagination(pagination).ToList();
                resultCount = result.Count;
            }
            else
            {
                result = query.ToList();
                resultCount = result.Count;
                totalResultCount = resultCount;
            }

            return Mapper.Map<IEnumerable<LoadTimerExpandedDto>>(result);
        }
        public IEnumerable<TruckerDailyDto> GetByDateRange(DateTime startDate, DateTime endDate, int? departmentID)
        {
            var result = _loadTimerRepository.GetTruckerDailies(startDate, endDate, departmentID).ToList();
            return result;         
        }

        public TruckerDailyDto GetLoadTruckerDaily(int loadTimerID)
        {
            var result = _loadTimerRepository.GetLoadTruckerDaily(loadTimerID);
            return result;
        }

    }

    public interface ILoadTimerService : IGenericService<LoadTimerDto>
    {
        LoadTimerDto InsertNativeApp(LoadTimerDto dto);
        LoadTimerPrimaryDto FindPrimary(int loadTimerID);
        IEnumerable<LoadTimerExpandedDto> GetAllExpanded(int timesheetID, NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount);
        IEnumerable<TruckerDailyDto> GetByDateRange(DateTime startDate, DateTime endDate, int? departmentID);
        TruckerDailyDto GetLoadTruckerDaily(int loadTimerID);
        void CheckJobPhaseCategoryConsistency(LoadTimerDto dto);
    }
}
