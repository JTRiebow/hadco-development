using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing.Conventions;
using AutoMapper;
using Hadco.Common.DataTransferObjects;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;
using Hadco.Common.Enums;

namespace Hadco.Services
{
    public class MappingProfile
    {
        public static void Configure()
        {
            Mapper.CreateMap<Resource, ResourceDto>().ReverseMap();
            Mapper.CreateMap<RoleResource, RoleResourceDto>().ReverseMap();
            Mapper.CreateMap<Role, RoleDto>().ReverseMap();

            Mapper.CreateMap<Employee, EmployeeDto>().ReverseMap();
            Mapper.CreateMap<Employee, BaseEmployeeDto>().ReverseMap();
            Mapper.CreateMap<Employee, SimpleEmployeeDto>();
            Mapper.CreateMap<Employee, PatchPostEmployeeDto>().ReverseMap();
            Mapper.CreateMap<Employee, ExpandedEmployeeDto>()
                .ForMember(dest => dest.IsClockedIn, opt => opt.MapFrom(src => src.ClockedInStatus != null ? src.ClockedInStatus.IsClockedIn : null as bool?)).ReverseMap();
            Mapper.CreateMap<PatchPostEmployeeDto, Employee>().ReverseMap();

            Mapper.CreateMap<EmployeeType, EmployeeTypeDto>().ReverseMap();

            Mapper.CreateMap<Customer, CustomerDto>().ReverseMap();

            Mapper.CreateMap<Department, DepartmentDto>().ReverseMap();

            Mapper.CreateMap<PatchDepartmentDto, Department>().ReverseMap();

            Mapper.CreateMap<Equipment, EquipmentDto>().ReverseMap();

            Mapper.CreateMap<Equipment, BaseEquipmentDto>().ReverseMap();

            Mapper.CreateMap<Job, JobDto>().ReverseMap();
            Mapper.CreateMap<Job, BaseJobDto>();
            Mapper.CreateMap<Phase, PhaseDto>().ReverseMap();
            Mapper.CreateMap<Category, CategoryDto>().ReverseMap();

            Mapper.CreateMap<Occurrence, OccurrenceDto>().ReverseMap();

            Mapper.CreateMap<TimesheetDto, Timesheet>().ReverseMap();
            Mapper.CreateMap<Timesheet, BaseTimesheetDto>().ReverseMap();
            Mapper.CreateMap<Timesheet, ExpandedTimesheetDto>()
                .ForMember(dest => dest.EquipmentTimers, opt => opt.PreCondition(src => src.EquipmentTimers != null))
                .ForMember(dest=>dest.EquipmentTimers, opt => opt.MapFrom(src => src.EquipmentTimers.SelectMany(x=>x.EquipmentTimerEntries)));

            Mapper.CreateMap<LoadTimerDto, LoadTimer>().ReverseMap();
            Mapper.CreateMap<LoadTimer, LoadTimerExpandedDto>().ReverseMap();
            Mapper.CreateMap<LoadTimer, LoadTimerPrimaryDto>().ReverseMap();


            Mapper.CreateMap<BillType, BillTypeDto>().ReverseMap();

            Mapper.CreateMap<DowntimeTimer, DowntimeTimerDto>().ReverseMap();
            Mapper.CreateMap<DowntimeTimer, DowntimeTimerExpandedDto>().ReverseMap();
            Mapper.CreateMap<DowntimeReason, DowntimeReasonDto>().ReverseMap();

            Mapper.CreateMap<Material, MaterialDto>().ReverseMap();

            Mapper.CreateMap<JobTimerDto, JobTimer>().ReverseMap();
            Mapper.CreateMap<JobTimer, ExpandedJobTimerDto>().ReverseMap();
            Mapper.CreateMap<JobTimer, ExpandedJobTimerFromJobTimerDto>()
                .ForMember(dest => dest.Supervisor, opt => opt.MapFrom(src => src.Timesheet.Employee)).ReverseMap();
            Mapper.CreateMap<JobTimer, ExpandedJobTimerWithEquipmentDto>()
                .ForMember(dest => dest.Supervisor, opt => opt.MapFrom(src => src.Timesheet.Employee)).ReverseMap();

            Mapper.CreateMap<EmployeeTimerDto, EmployeeTimer>()
                .ForMember(dest => dest.AuthorizeNote, opt => opt.MapFrom(source =>
                !string.IsNullOrWhiteSpace(source.AuthorizeNote) ? new Note()
                {
                    EmployeeID = source.EmployeeID,
                    Day = source.Day,
                    DepartmentID = source.DepartmentID,
                    Description = source.AuthorizeNote,
                    NoteTypeID = (int)NoteTypeName.AuthorizeNote,
                    CreatedTime = DateTimeOffset.Now,
                    CreatedEmployeeID = source.EmployeeID
                } : null))
                .ReverseMap()
                .ForMember(dest => dest.AuthorizeNote, opt => opt.MapFrom(source => source.AuthorizeNote.Description));

            Mapper.CreateMap<EmployeeTimer, ExpandedEmployeeTimerDto>()
                .ForMember(dest => dest.Supervisor, opt => opt.MapFrom(src => src.Timesheet != null ? src.Timesheet.Employee : null)).ReverseMap();

            Mapper.CreateMap<EmployeeTimer, ForemanEmployeeTimer>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.Name));
            Mapper.CreateMap<ForemanEmployeeTimerPatch, EmployeeTimer>()
                .ForMember(dest => dest.EmployeeJobTimers, opt => opt.MapFrom(src => src.EmployeeJobTimers))
                .ReverseMap();
            Mapper.CreateMap<ForemanEmployeeJobTimer, EmployeeJobTimer>()
                .ForMember(dest => dest.EmployeeJobEquipmentTimers,
                    opt => opt.MapFrom(src => src.EmployeeJobEquipmentTimers)).ReverseMap();
            Mapper.CreateMap<ForemanEmployeeJobTimerPatch, EmployeeJobTimer>().ReverseMap();
            Mapper.CreateMap<ForemanEmployeeJobEquipmentTimerPatch, EmployeeJobEquipmentTimer>().ReverseMap();

            Mapper.CreateMap<EmployeeTimerEntry, EmployeeTimerEntryDto>();

            Mapper.CreateMap<Note, String>().ConstructProjectionUsing(x => x.Description);

            Mapper.CreateMap<EmployeeTimerEntryDto, EmployeeTimerEntry>()
                .ForMember(dest => dest.ClockInNote, opt => opt.Ignore())
                .ForMember(dest => dest.ClockOutNote, opt => opt.Ignore());

            Mapper.CreateMap<EmployeeTimerEntry, ExpandedEmployeeTimerEntryDto>().ReverseMap();
            Mapper.CreateMap<EmployeeTimerEntryHistory, EmployeeTimerEntryHistoryDto>().ReverseMap();


            Mapper.CreateMap<EmployeeJobTimerDto, EmployeeJobTimer>().ReverseMap();
            Mapper.CreateMap<EmployeeJobTimerExpandedDto, EmployeeJobTimer>()
                .ForMember(dest => dest.EmployeeJobEquipmentTimers,
                    opt => opt.MapFrom(src => src.EmployeeJobEquipmentTimers))
                    .ReverseMap();
            Mapper.CreateMap<EmployeeJobTimer, EmployeeJobTimerPrimaryDto>()
                .ForMember(dest => dest.Supervisor, opt => opt.MapFrom(src => src.EmployeeTimer.Timesheet.Employee)).ReverseMap();
            Mapper.CreateMap<EmployeeJobTimer, EmployeeJobTimerFromJobTimerDto>()
                .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.EmployeeTimer.Employee)).ReverseMap();

            Mapper.CreateMap<EmployeeJobTimer, ExpandedJobTimerFromEmployeeTimerDto>().ReverseMap();

            Mapper.CreateMap<EmployeeJobEquipmentTimer, EmployeeJobEquipmentTimerDto>().ReverseMap();
            Mapper.CreateMap<EmployeeJobEquipmentTimer, EmployeeJobEquipmentTimerExpandedDto>().ReverseMap();

            Mapper.CreateMap<EquipmentTimer, EquipmentTimerDto>().ReverseMap();
            Mapper.CreateMap<EquipmentTimer, ExpandedEquipmentTimerDto>().ReverseMap();

            Mapper.CreateMap<EquipmentTimer, ExpandedEquipmentTimerEntryDto>()
                  .ForMember(dest => dest.EquipmentTimerID, opt => opt.MapFrom(src => src.ID))
                  .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.EquipmentTimerEntryID)).ReverseMap();

            Mapper.CreateMap<EquipmentTimerDto, EquipmentTimerEntry>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.EquipmentTimerEntryID))
                .ForMember(dest => dest.EquipmentTimerID, opt => opt.MapFrom(src => src.ID));


            Mapper.CreateMap<EquipmentTimerEntry, EquipmentTimerEntryDto>().ReverseMap();

            Mapper.CreateMap<EquipmentTimerEntry, ExpandedEquipmentTimerEntryDto>()
                .ForMember(dest => dest.Equipment, opt => opt.MapFrom(src => src.EquipmentTimer.Equipment))
                .ForMember(dest => dest.EquipmentID, opt => opt.MapFrom(src => src.EquipmentTimer.EquipmentID))
                .ForMember(dest => dest.TimesheetID, opt => opt.MapFrom(src => src.EquipmentTimer.TimesheetID)).ReverseMap();

            Mapper.CreateMap<EquipmentTimerDto, EquipmentTimerEntryDto>().ReverseMap();

            Mapper.CreateMap<EmployeeTimecard, EmployeeTimecardDto>().ReverseMap();
            Mapper.CreateMap<EmployeeTimecard, ExpandedEmployeeTimecardDto>().ReverseMap();

            Mapper.CreateMap<EquipmentServiceType, EquipmentServiceTypeDto>().ReverseMap();

            Mapper.CreateMap<TruckClassification, TruckClassificationDto>().ReverseMap();

            Mapper.CreateMap<Price, PriceDto>().ReverseMap();
            Mapper.CreateMap<Price, BasePriceDto>();
            Mapper.CreateMap<Price, ExpandedPriceDto>().ReverseMap();

            Mapper.CreateMap<Pricing, PricingDto>().ReverseMap();
            Mapper.CreateMap<Pricing, ExpandedPricingDto>().ReverseMap();

            Mapper.CreateMap<Pit, PitDto>().ReverseMap();

            Mapper.CreateMap<LoadTimer, TransportLoadTimerDto>().ReverseMap();

            Mapper.CreateMap<LoadTimerDto, LoadTimerEntryDto>()
                .ConstructUsing(x => new LoadTimerEntryDto()
                {
                    LoadTimerID = x.ID,
                    StartTime = x.LoadTime,
                    StartTimeLatitude = x.LoadTimeLatitude,
                    StartTimeLongitude = x.LoadTimeLongitude,
                    EndTime = x.DumpTime,
                    EndTimeLatitude = x.DumpTimeLatitude,
                    EndTimeLongitude = x.LoadTimeLongitude
                }).ReverseMap();

            Mapper.CreateMap<LoadTimerDto, LoadTimerEntry>()
                .ConstructUsing(x => new LoadTimerEntry()
                {
                    LoadTimerID = x.ID,
                    StartTime = x.LoadTime,
                    StartTimeLatitude = x.LoadTimeLatitude,
                    StartTimeLongitude = x.LoadTimeLongitude,
                    EndTime = x.DumpTime,
                    EndTimeLatitude = x.DumpTimeLatitude,
                    EndTimeLongitude = x.LoadTimeLongitude
                }).ReverseMap();

            Mapper.CreateMap<LoadTimerEntry, LoadTimerEntryDto>().ReverseMap();

            Mapper.CreateMap<Location, LocationDto>().ReverseMap();

            Mapper.CreateMap<Setting, SettingDto>().ReverseMap();
            Mapper.CreateMap<PostSettingDto, Setting>();

            Mapper.CreateMap<NoteDto, Note>()
                .ForMember(dest => dest.NoteTypeID, opt => opt.MapFrom(src => (int)src.NoteTypeID)).ReverseMap();

            Mapper.CreateMap<NoteTypeDto, NoteType>().ReverseMap();

            Mapper.CreateMap<DailyApproval, BaseDailyApprovalDto>().ReverseMap();
            Mapper.CreateMap<DailyApproval, DailyApprovalDto>().ReverseMap();
            Mapper.CreateMap<DailyApproval, DailyApprovalPatchDto>().ReverseMap();

            Mapper.CreateMap<RoleAuthActivity, RoleAuthActivityDto>()
                .ForSourceMember(src => src.AuthActivity, opts => opts.Ignore())
                .ForSourceMember(src => src.Role, opts => opts.Ignore())
                .ForMember(dest => dest.DepartmentIds, opt => opt.MapFrom(src => src.Departments.Select(x => x.ID)));

            Mapper.CreateMap<RoleAuthActivityDto, RoleAuthActivity>()
                .ForSourceMember(src => src.DepartmentIds, opt => opt.Ignore());

            Mapper.CreateMap<AuthActivity, AuthActivityDto>()
                .ForMember(dest => dest.AuthSection, opt => opt.MapFrom(src => src.AuthSection));

            Mapper.CreateMap<AuthSection, AuthSectionDto>().ReverseMap();
        }
    }
}