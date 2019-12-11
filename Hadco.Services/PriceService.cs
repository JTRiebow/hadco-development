using AutoMapper;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;
using Hadco.Common.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;
using Hadco.Common.Enums;

namespace Hadco.Services
{
    public class PriceService : GenericService<PriceDto, Price>, IPriceService
    {
        private IPriceRepository _priceRepository;
        private IPricingRepository _pricingRepository;
        private ILoadTimerRepository _loadTimerRepository;

        public PriceService(IPriceRepository priceRepository, IPricingRepository pricingRepository, ILoadTimerRepository loadTimerRepository,
                            IPrincipal currentUser) : base(priceRepository, currentUser)
        {
            _priceRepository = priceRepository;
            _pricingRepository = pricingRepository;
            _loadTimerRepository = loadTimerRepository;
        }

        public override PriceDto Insert(PriceDto dto)
        {
            var priceDto = base.Insert(dto);
            var pricing = _pricingRepository.FindNoTracking(priceDto.PricingID);
            _loadTimerRepository.UpdatePricePerUnit(pricing.CustomerTypeID, pricing.BillTypeID, pricing.JobID, pricing.CustomerID, pricing.PhaseID, dto.MaterialID, dto.TruckClassificationID);
            return priceDto;
        }

        public dynamic GetPricesByID(int pricingID, out int resultCount, out int totalResultCount)
        {
            var results = (IEnumerable<dynamic>)_pricingRepository.GetPricesByID(pricingID);
            resultCount = results.Count();
            totalResultCount = resultCount;
            return results;
        }
    }

    public interface IPriceService : IGenericService<PriceDto>
    {
        dynamic GetPricesByID(int pricingID, out int resultCount, out int totalResultCount);
    }
}
