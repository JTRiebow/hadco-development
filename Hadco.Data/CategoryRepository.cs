using Hadco.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace Hadco.Data
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {               
        public IEnumerable<Category> GetHadcoShop()
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                return sc.Query<Category>(@"
                                      SELECT CategoryID ID, CategoryNumber, JobNumber, PhaseNumber, PhaseID, Name, JobID
                                      FROM [dbo].Categories
                                      where jobnumber = 'HADCO'
                                      and PhaseNumber = 'SHOP'
                                      and CategoryNumber in ('COUR', 'MTG', 'TRUCK', 'YARD', 'SHOP', 'PREP', 'SMENG')
                ");                
            }

        }

        public Category GetHadcoShopOverhead()
        {
            using (var sc = new SqlConnection(Context.Database.Connection.ConnectionString))
            {
                sc.Open();
                return sc.Query<Category>(@"
                                      SELECT CategoryID ID, CategoryNumber, JobNumber, PhaseNumber, PhaseID, Name, JobID
                                      FROM [dbo].Categories
                                      where jobnumber = 'HADCO'
                                      and PhaseNumber = 'SHOP'
                                      and CategoryNumber in ('PREP')
                ").FirstOrDefault();
            }

        }
    }

    public interface ICategoryRepository : IGenericRepository<Category>
    {
        IEnumerable<Category> GetHadcoShop();
        Category GetHadcoShopOverhead();

    }
}
