using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Principal;
using Hadco.Common;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;

namespace Hadco.Services
{
    public class DowntimeReasonService : GenericService<DowntimeReasonDto, DowntimeReason>, IDowntimeReasonService
    {
        private IDowntimeReasonRepository _downtimeReasonRepository;
        public DowntimeReasonService(IDowntimeReasonRepository downtimeReasonRepository, IPrincipal currentUser)
            : base(downtimeReasonRepository, currentUser)
        {
            _downtimeReasonRepository = downtimeReasonRepository;
        }

        public override IEnumerable<DowntimeReasonDto> GetAll(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            return base.GetAll(filter, pagination, out resultCount, out totalResultCount);
        }
    }

    public interface IDowntimeReasonService : IGenericService<DowntimeReasonDto>
    {

    }
}
