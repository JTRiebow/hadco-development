using AutoMapper;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Principal;
using Hadco.Data.Entities;
using Hadco.Data;
using Hadco.Common;
using Hadco.Services.DataTransferObjects;

namespace Hadco.Services
{
    public abstract class GenericService<D, E> : IGenericService<D> where E : IModel where D : IDataTransferObject
    {
        private IGenericRepository<E> _genericRepository;
        protected GenericService(IGenericRepository<E> genericRepository, IPrincipal currentUser)
        {
            _genericRepository = genericRepository;
            CurrentUser = (ClaimsPrincipal)currentUser;
        }
                
        protected ClaimsPrincipal CurrentUser
        {
            get;
            private set;
        }

        public virtual IEnumerable<D> All
        {
            get
            {
                return Mapper.Map<IEnumerable<E>, IEnumerable<D>>(_genericRepository.All);
            }
        }

        public virtual IEnumerable<D> GetAll(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount)
        {
            IQueryable<E> query;
            if (filter != null)
            {
                query = _genericRepository.All.Filter<E>(filter);
            }
            else
            {
                query = _genericRepository.All;
            }

            // if there is pagination then run the query twice, else convert to a list to get the count so that the query only runs once.
            List<E> result;
            if (pagination != null)
            {
                totalResultCount = query.Count();
                result = query.Pagination<E>(pagination).ToList<E>();
                resultCount = result.Count;
            }
            else
            {
                result = query.ToList<E>();
                resultCount = result.Count;
                totalResultCount = resultCount;
            }

            return Mapper.Map<IEnumerable<D>>(result);
        }

        public virtual D Find(int id, bool tracking = true)
        {
            E item;

            if (tracking)
                item = _genericRepository.Find(id);
            else
                item = _genericRepository.FindNoTracking(id);

            return Mapper.Map<E, D>(item);
        }

        public virtual bool Exists(int id)
        {
            return _genericRepository.Exists(id);
        }

        public virtual D Insert(D dto)
        {
            return Mapper.Map<D>(_genericRepository.Insert(Mapper.Map<E>(dto)));
        }

        public virtual D Update(D dto)
        {
            E entity = _genericRepository.Find(dto.ID);
            Mapper.Map<D, E>(dto, entity);
            _genericRepository.Save();
            return Mapper.Map<D>(entity);
        }

        public virtual void Delete(int id)
        {
            _genericRepository.Delete(id);
        }
    }

    public interface IGenericService<D>
    {
        IEnumerable<D> All { get; }
        void Delete(int id);
        D Find(int id, bool tracking = true);
        D Insert(D dto);
        D Update(D dto);
        bool Exists(int id);
        IEnumerable<D> GetAll(NameValueCollection filter, Pagination pagination, out int resultCount, out int totalResultCount);
    }
}
