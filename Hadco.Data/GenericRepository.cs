using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data
{
    public class GenericRepository<E> : IGenericRepository<E> where E : class, IModel
    {
        public GenericRepository()
        {
        }

        private HadcoContext _context;
        protected HadcoContext Context
        {
            get
            {
                return _context = _context ?? new HadcoContext();
            }
        }

        private IDbSet<E> _entities;
        protected IDbSet<E> Entities
        {
            get
            {
                return _entities = _entities ?? Context.Set<E>();
            }
        }

        public virtual IQueryable<E> All
        {
            get
            {
                return Entities;
            }
        }

        public IQueryable<E> AllIncluding(params Expression<Func<E, object>>[] includeProperties)
        {
            IQueryable<E> query = All;
            foreach (var includeProperty in includeProperties)
                query = query.Include(includeProperty);

            return query;
        }

        public virtual bool Exists(int id)
        {
            return FindNoTracking(id) != null;
        }

        public virtual E Find(int id, params Expression<Func<E, object>>[] includeProperties)
        {
            return AllIncluding(includeProperties).FirstOrDefault(x => x.ID == id);
        }

        public virtual E FindNoTracking(int id, params Expression<Func<E, object>>[] includeProperties)
        {
            return AllIncluding(includeProperties).AsNoTracking().FirstOrDefault(x => x.ID == id);
        }

        public virtual E Insert(E entity)
        {
            Entities.Add(entity);
            Save();

            return entity;
        }

        public virtual E Update(E entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Save();
            return entity;
        }

        public virtual bool Delete(int id)
        {
            var entity = Find(id);

            if (entity == null)
                return false;

            Entities.Remove(entity);
            Save();
            return true;
        }

        public virtual void Save()
        {
            try
            {
                Context.Database.Connection.Open();
                Context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var error in ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors))
                    Trace.TraceInformation("{0}DbEntityValidationException - Property: {1} Error: {2}", Environment.NewLine, error.PropertyName, error.ErrorMessage);

                throw;
            }
            finally
            {
                if (Context.Database.Connection.State == ConnectionState.Open)
                    Context.Database.Connection.Close();
            }
        }
    }

    public interface IGenericRepository<E>
    {
        IQueryable<E> All { get; }
        IQueryable<E> AllIncluding(params Expression<Func<E, object>>[] includeProperties);
        E Find(int id, params Expression<Func<E, object>>[] includeProperties);
        E FindNoTracking(int id, params Expression<Func<E, object>>[] includeProperties);
        E Insert(E entity);
        E Update(E entity);
        bool Delete(int id);
        bool Exists(int id);
        void Save();
    }
}
