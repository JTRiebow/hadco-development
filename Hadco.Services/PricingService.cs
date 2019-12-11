using AutoMapper;
using Hadco.Data;
using Hadco.Data.Entities;
using Hadco.Services.DataTransferObjects;
using Hadco.Common.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Hadco.Common;
using Hadco.Common.Enums;
using Ninject.Activation;

namespace Hadco.Services
{
    public class PricingService : GenericService<PricingDto, Pricing>, IPricingService
    {
        private IPricingRepository _pricingRepository;
        private IJobRepository _jobRepository;
        private ITruckClassificationRepository _truckClassificationRepository;

        public PricingService(IPricingRepository pricingRepository,
                                IJobRepository jobRepository,
                                ITruckClassificationRepository truckClassificationRepository,
                                IPrincipal currentUser) : base(pricingRepository, currentUser)
        {
            _pricingRepository = pricingRepository;
            _jobRepository = jobRepository;
            _truckClassificationRepository = truckClassificationRepository;
        }

        public override PricingDto Insert(PricingDto dto)
        {
            dto.StartDate = dto.StartDate.Date; 
            CheckPricingCombination(dto.CustomerTypeID, dto.JobID, dto.CustomerID, dto.PhaseID);
            ClosePreviousPricing(dto);
            dto.UpdatedTime = DateTimeOffset.Now;
            dto =  base.Insert(dto);
            return dto;

        }

        public dynamic GetPriceGrid(int customerTypeID, int billTypeID)
        {
            var grid = billTypeID == (int)BillTypeName.Hourly ? _pricingRepository.GetTruckPrices(customerTypeID, billTypeID) : _pricingRepository.GetMaterialPrices(customerTypeID, billTypeID);
            return grid;
        }

        public dynamic GetCurrentPricing(int pricingID)
        {
            var pricing = _pricingRepository.AllIncluding(x => x.Customer, x => x.Job, x => x.Phase)?.FirstOrDefault(x => x.ID == pricingID);

            if (pricing == null) return null;

            var expandedPricing = Mapper.Map<ExpandedPricingDto>(pricing);
            var prices = Mapper.DynamicMap<ICollection<ExpandedPriceDto>>(_pricingRepository.GetPricesByID(pricingID));

            expandedPricing.Prices = prices;

            return expandedPricing;
        }

        public dynamic GetCustomerJobOrPhaseList(int customerTypeID, int billTypeID)
        {
            switch (customerTypeID)
            {
                case (int)CustomerTypeName.Development:
                    return _pricingRepository.GetJobPricingList(customerTypeID, billTypeID);
                case (int)CustomerTypeName.Residential:
                    return _pricingRepository.GetCustomerPricingList(customerTypeID, billTypeID);
                case (int)CustomerTypeName.Outside:
                    return _pricingRepository.GetPhasePricingList(customerTypeID, billTypeID);
                case (int)CustomerTypeName.Metro:
                    return _pricingRepository.GetPhasePricingList(customerTypeID, billTypeID);
                default:
                    return null;
            }
        }

        public decimal? FindPrice(LoadTimerDto loadTimer)
        {
            if (loadTimer?.JobID == null || loadTimer?.BillTypeID == null)
                return null;

            if (loadTimer.TruckClassificationID == null)
            {
                var truckClassificationID = _truckClassificationRepository.GetTruckClassificationID(loadTimer.TruckID, loadTimer.TrailerID, loadTimer.PupID);
                loadTimer.TruckClassificationID = truckClassificationID;
            }
            var customerTypeID = _jobRepository.FindNoTracking((int)loadTimer.JobID).CustomerTypeID;
            if (customerTypeID == null)
                return null;

            int? jobID = null;
            int? customerID = null;
            int? phaseID = null;

            switch (customerTypeID)
            {
                case (int)CustomerTypeName.Development:
                    jobID = loadTimer.JobID;
                    break;
                case (int)CustomerTypeName.Residential:
                    customerID = _jobRepository.FindNoTracking(loadTimer.JobID.Value).CustomerID;
                    break;
                case (int)CustomerTypeName.Outside:
                case (int)CustomerTypeName.Metro:
                    phaseID = loadTimer.PhaseID;
                    break;
            }

            var pricing = _pricingRepository.AllIncluding(x => x.Prices).OrderByDescending(x=>x.UpdatedTime)
                                            .FirstOrDefault(x => x.BillTypeID == loadTimer.BillTypeID
                                              && x.CustomerTypeID == customerTypeID
                                              && x.JobID == jobID
                                              && x.CustomerID == customerID
                                              && x.PhaseID == phaseID
                                              && x.StartDate < loadTimer.LoadTime
                                              && (x.EndDate == null || x.EndDate > loadTimer.DumpTime));

            switch (loadTimer.BillTypeID)
            {
                case (int)BillTypeName.Hourly:
                    {
                        var price = pricing?.Prices?.SingleOrDefault(x => x.TruckClassificationID == loadTimer.TruckClassificationID)?.Value;
                        return price;
                    }
                case (int)BillTypeName.Ton:
                case (int)BillTypeName.Load:
                    {
                        var price = pricing?.Prices?.SingleOrDefault(x => x.MaterialID == loadTimer.MaterialID)?.Value;
                        return price;
                    }
                default:
                    return null;
            }
        }

        public dynamic GetPriceHistory(int customerTypeID, int billTypeID, int? jobID = null, int? customerID = null, int? phaseID = null)
        {
            CheckPricingCombination(customerTypeID, jobID, customerID, phaseID);

            if (billTypeID == (int)BillTypeName.Hourly)
            {
                return (IEnumerable<dynamic>)_pricingRepository.GetHistoryTrucks(customerTypeID, billTypeID, jobID, customerID, phaseID);
            }
            return (IEnumerable<dynamic>)_pricingRepository.GetHistoryMaterials(customerTypeID, billTypeID, jobID, customerID, phaseID);
        }

        public bool IsOverlapping(PricingDto dto)
        {
             return _pricingRepository.All.Any(x => x.CustomerTypeID == dto.CustomerTypeID
                                              && x.BillTypeID == dto.BillTypeID
                                              && x.JobID == dto.JobID
                                              && x.CustomerID == dto.CustomerID
                                              && x.PhaseID == dto.PhaseID
                                              && ((x.StartDate <= dto.StartDate || ( !x.EndDate.HasValue || dto.StartDate <= x.EndDate))
                                                || (x.StartDate >= dto.EndDate) && !(dto.EndDate.HasValue || dto.EndDate <= x.EndDate)));
        }

        private void ClosePreviousPricing(PricingDto dto)
        {
            var oldPricing = _pricingRepository.All.Where(x => x.CustomerTypeID == dto.CustomerTypeID
                                                               && x.BillTypeID == dto.BillTypeID
                                                               && x.JobID == dto.JobID
                                                               && x.CustomerID == dto.CustomerID
                                                               && x.PhaseID == dto.PhaseID
                                                               && x.StartDate <= dto.StartDate).OrderByDescending(x => x.UpdatedTime).FirstOrDefault();
            if (oldPricing == null) return;
            if (oldPricing.EndDate != null) return;

            oldPricing.EndDate = dto.StartDate;
            _pricingRepository.Update(oldPricing);
        }
        private static void CheckPricingCombination(int customerTypeID, int? jobID, int? customerID, int? phaseID)
        {
            switch (customerTypeID)
            {
                case (int)CustomerTypeName.Development:
                    if (jobID == null) throw new ArgumentException("Developmental pricing requires a Job ID");
                    break;
                case (int)CustomerTypeName.Residential:
                    if (customerID == null) throw new ArgumentException("Residential pricing requires a Customer ID");
                    break;
                case (int)CustomerTypeName.Outside:
                    if (phaseID == null) throw new ArgumentException("Outside pricing requires a Phase ID");
                    break;
                case (int)CustomerTypeName.Metro:
                    if (phaseID == null) throw new ArgumentException("Metro pricing requires a Phase ID");
                    break;
            }
        }
    }

    public interface IPricingService : IGenericService<PricingDto>
    {
        bool IsOverlapping(PricingDto dto);
        decimal? FindPrice(LoadTimerDto loadTimer);
        dynamic GetPriceGrid(int customerTypeID, int billTypeID);
        dynamic GetCurrentPricing(int pricingID);
        dynamic GetCustomerJobOrPhaseList(int customerTypeID, int billTypeID);
        dynamic GetPriceHistory(int customerTypeID, int billTypeID, int? jobID, int? customerID, int? phaseID);
    }

}
