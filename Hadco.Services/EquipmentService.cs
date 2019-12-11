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
using System.Collections.Specialized;
using Hadco.Common;

namespace Hadco.Services
{
    public class EquipmentService : GenericService<EquipmentDto, Equipment>, IEquipmentService
    {
        private IEquipmentRepository _equipmentRepository;
        public EquipmentService(IEquipmentRepository equipmentRepository, IPrincipal currentUser)
            : base(equipmentRepository, currentUser)
        {
            _equipmentRepository = equipmentRepository;
        }

        public IEnumerable<EquipmentDto> GetTrucks(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            var truckCodes = new[] { "DT", "S", "LT", "ST", "WT", "SW", "VT", "P", "CT"};
            var equipmentNumbers = new[] { "P697", "P713", "P691", "P689" };
            IQueryable<Equipment> query;
            if (filter != null)
            {
                query = _equipmentRepository.All.Filter(filter);
            }
            else
            {
                query = _equipmentRepository.All;
            }

            query = query.Where(x => (truckCodes.Contains(x.Type) || equipmentNumbers.Contains(x.EquipmentNumber)) && x.Status == EntityStatus.Active);

            // if there is pagination then run the query twice, else convert to a list to get the count so that the query only runs once.
            List<Equipment> result;
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

            return Mapper.Map<IEnumerable<EquipmentDto>>(result);
        }

        public IEnumerable<EquipmentDto> GetTrailers(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            var trailerCodes = new[] { "BD", "ED", "SD", "PT", "T" };
            IQueryable<Equipment> query;
            if (filter != null)
            {
                query = _equipmentRepository.All.Filter(filter);
            }
            else
            {
                query = _equipmentRepository.All;
            }

            query = query.Where(x => trailerCodes.Contains(x.Type) && x.Status == EntityStatus.Active);

            // if there is pagination then run the query twice, else convert to a list to get the count so that the query only runs once.
            List<Equipment> result;
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

            return Mapper.Map<IEnumerable<EquipmentDto>>(result);
        }

    }

    public interface IEquipmentService : IGenericService<EquipmentDto>
    {
        IEnumerable<EquipmentDto> GetTrucks(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount);
        IEnumerable<EquipmentDto> GetTrailers(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount);
    }
}
