using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Principal;
using Geocoding.Microsoft;
using Hadco.Common.DataTransferObjects;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;
using Hadco.Common;
using System.Collections.Specialized;

namespace Hadco.Services
{
	public class JobService : GenericService<JobDto, Job>, IJobService
    {
	    private IJobRepository _jobRepository;
        public JobService(IJobRepository jobRepository, IPrincipal currentUser)
            : base(jobRepository, currentUser)
		{
			_jobRepository = jobRepository;
		}

        public IEnumerable<JobDto> GetAllWithDateFilter(DateTimeOffset? lastUpdated, NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<Job> query;
            if (filter != null)
            {
                query = _jobRepository.All.Filter(filter);
            }
            else
            {
                query = _jobRepository.All;
            }

            if (lastUpdated != null)
            {
                DateTimeOffset lastUpdatedValue = lastUpdated.Value;
                query = query.Where(x => x.ModifiedDate >= lastUpdatedValue);
            }

            query = query.OrderBy(x => x.JobNumber);

            // if there is pagination then run the query twice, else convert to a list to get the count so that the query only runs once.
            List<Job> result;
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

            return Mapper.Map<IEnumerable<JobDto>>(result);
        }
    }

	public interface IJobService : IGenericService<JobDto>
	{
        IEnumerable<JobDto> GetAllWithDateFilter(DateTimeOffset? lastUpdated, NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount);
    }
}
