using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public IQueryable<RoleResource> GetAllRoleResources()
        {
            return Context.RoleResources
                .Include(x => x.Role)
                .Include(x => x.Resource);
        }

        public RoleResource AddResource(int id, RoleResource entity)
        {
            entity.RoleID = id;
            Context.RoleResources.Add(entity);
            Save();
            return GetRoleResource(id, entity.ResourceID);
        }

        private RoleResource GetRoleResource(int roleID, int resourceID)
        {
            return GetAllRoleResources()
                .Where(x => x.RoleID == roleID && x.ResourceID == resourceID)
                .FirstOrDefault();
        }

        public void RemoveResource(int id, int resourceID)
        {
            var roleResourceItem = Context.RoleResources.Where(x => x.RoleID == id && x.ResourceID == resourceID).FirstOrDefault();
            Context.RoleResources.Remove(roleResourceItem);
            Save();
        }

        public bool RoleResourceExists(int id, int resourceID)
        {
            return Context.RoleResources.Any(x => x.RoleID == id && x.ResourceID == resourceID);
        }

        public RoleResource UpdateResourcePermissionForRole(int id, RoleResource entity)
        {
            entity.RoleID = id;
            Context.Entry(entity).State = EntityState.Modified;
            Save();
            return GetRoleResource(id, entity.ResourceID);
        }
    }

    public interface IRoleRepository : IGenericRepository<Role>
    {
        void RemoveResource(int id, int resourceID);
        RoleResource AddResource(int id, RoleResource entity);
        bool RoleResourceExists(int id, int resourceID);
        RoleResource UpdateResourcePermissionForRole(int id, RoleResource entity);
        IQueryable<RoleResource> GetAllRoleResources();
    }
}
