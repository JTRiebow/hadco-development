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
	public class TruckClassificationService : GenericService<TruckClassificationDto, TruckClassification>, ITruckClassificationService
    {
	    private ITruckClassificationRepository _truckClassificationRepository;
        public TruckClassificationService(ITruckClassificationRepository truckClassificationRepository, IPrincipal currentUser)
            : base(truckClassificationRepository, currentUser)
		{
            _truckClassificationRepository = truckClassificationRepository;
		}
	}

	public interface ITruckClassificationService : IGenericService<TruckClassificationDto>
	{

	}
}
