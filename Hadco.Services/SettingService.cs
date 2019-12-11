using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;
using AutoMapper;

namespace Hadco.Services
{
    public class SettingService : GenericService<SettingDto, Setting>, ISettingService
    {
        ISettingRepository _settingRepository;
        public SettingService(ISettingRepository settingRepository, IPrincipal currentUser)
            : base(settingRepository, currentUser)
        {
            _settingRepository = settingRepository;
        }

        public SettingDto Insert(PostSettingDto dto)
        {
            var setting = Mapper.Map<Setting>(dto);
            setting.ModifiedTime = DateTimeOffset.Now;
            return Mapper.Map<SettingDto>(_settingRepository.Insert(setting));
        }

        public int GetBreadCrumbIntervalInSeconds()
        {
            return _settingRepository.GetBreadCrumbIntervalInSeconds();
        }
    }

    public interface ISettingService : IGenericService<SettingDto>
    {
        SettingDto Insert(PostSettingDto dto);
        int GetBreadCrumbIntervalInSeconds();
    }
}
