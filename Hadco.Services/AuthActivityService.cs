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
using Hadco.Common.Enums;

namespace Hadco.Services
{
    public class AuthActivityService : GenericService<AuthActivityDto, AuthActivity>, IAuthActivityService
    {
        private IAuthActivityRepository _authActivityRepository;

        public AuthActivityService(IAuthActivityRepository authActivityRepository, IPrincipal currentUser) 
            : base(authActivityRepository, currentUser)
        {
            _authActivityRepository = authActivityRepository;
        }

        public IEnumerable<AuthActivityDto> GetAllAuthActivities()
        {
            return Mapper.Map<IEnumerable<AuthActivityDto>>(_authActivityRepository.All);
        }

        public IEnumerable<AuthActivityDto> GetAuthActivitiesBySectionID(int authSectionID)
        {
            return Mapper.Map<IEnumerable<AuthActivityDto>>(_authActivityRepository.All.Where(x => (int)x.AuthSectionID == authSectionID));
        }

        public IEnumerable<int> GetAuthActivityIdsBySectionID(int authSectionID)
        {
            return _authActivityRepository.All.Where(x => (int)x.AuthSectionID == authSectionID).Select(x => x.ID).ToList();
        }
    }

    public interface IAuthActivityService : IGenericService<AuthActivityDto>
    {
        IEnumerable<AuthActivityDto> GetAllAuthActivities();
        IEnumerable<AuthActivityDto> GetAuthActivitiesBySectionID(int authSectionID);
        IEnumerable<int> GetAuthActivityIdsBySectionID(int authSectionID);
    }
}
