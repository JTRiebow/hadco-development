using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using System.Security.Principal;
using Hadco.Data;
using Hadco.Services.DataTransferObjects;
using Hadco.Data.Entities;

namespace Hadco.Services
{
	public class PhaseService : GenericService<PhaseDto, Phase>, IPhaseService
    {
	    private IPhaseRepository _phaseRepository;
        public PhaseService(IPhaseRepository phaseRepository, IPrincipal currentUser)
            : base(phaseRepository, currentUser)
		{
			_phaseRepository = phaseRepository;
		}

	    public IEnumerable<PhaseDto> GetMetroPhases()
	    {
	        return Mapper.Map<IEnumerable<PhaseDto>>(_phaseRepository.All.Where(x=>x.JobNumber == "METRO"));
	    }
    }

	public interface IPhaseService : IGenericService<PhaseDto>
	{
	    IEnumerable<PhaseDto> GetMetroPhases();
	}
}
