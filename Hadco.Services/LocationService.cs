using System;
using System.Security.Principal;
using Hadco.Common.DataTransferObjects;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;
using System.Collections.Generic;
using AutoMapper;

namespace Hadco.Services
{
    public class LocationService : ILocationService
    {
        private ILocationRepository _locationRepository;
        private readonly ISettingRepository _settingRepository;

        public LocationService(ILocationRepository locationRepository, 
            ISettingRepository settingRepository,
            IPrincipal currentUser)
        {
            _locationRepository = locationRepository;
            _settingRepository = settingRepository;
        }

        public IEnumerable<GPSCoordinates> Get(int employeeID, DateTimeOffset startTime, DateTimeOffset endTime, int? departmentID)
        {
            return _locationRepository.Get(employeeID, startTime, endTime, departmentID);
        }

        public GPSCoordinates GetMostRecentLocation(int employeeID)
        {
            return _locationRepository.GetMostRecentCoordinates(employeeID);
        }

        public LocationPostResponseDto Insert(LocationDto dto)
        {
            _locationRepository.Insert(Mapper.Map<Location>(dto));
            var interval = _settingRepository.GetBreadCrumbIntervalInSeconds();
            return new LocationPostResponseDto()
                   {
                       TimeToSendNextLocation = dto.TimeStamp.AddSeconds(interval)
                   };
        }
    }

    public interface ILocationService
    {
        GPSCoordinates GetMostRecentLocation(int employeeID);
        IEnumerable<GPSCoordinates> Get(int employeeID, DateTimeOffset startTime, DateTimeOffset endTime, int? departmentID);
        LocationPostResponseDto Insert(LocationDto dto);
    }
}