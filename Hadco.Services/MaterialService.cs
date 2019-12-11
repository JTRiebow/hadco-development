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
	public class MaterialService : GenericService<MaterialDto, Material>, IMaterialService
    {
	    private IMaterialRepository _materialRepository;
        public MaterialService(IMaterialRepository materialRepository, IPrincipal currentUser)
            : base(materialRepository, currentUser)
		{
			_materialRepository = materialRepository;
		}
	}

	public interface IMaterialService : IGenericService<MaterialDto>
	{

	}
}
