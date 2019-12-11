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
	public class CategoryService : GenericService<CategoryDto, Category>, ICategoryService
    {
	    private ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository, IPrincipal currentUser)
            : base(categoryRepository, currentUser)
		{
			_categoryRepository = categoryRepository;
		}

        public IEnumerable<CategoryDto> GetHadcoShop()
        {
            return Mapper.Map<IEnumerable<CategoryDto>>(_categoryRepository.GetHadcoShop());
        }

        public CategoryDto GetHadcoShopOverhead()
        {
            return Mapper.Map<CategoryDto>(_categoryRepository.GetHadcoShopOverhead());
        }
    }

	public interface ICategoryService : IGenericService<CategoryDto>
	{
        IEnumerable<CategoryDto> GetHadcoShop();
        CategoryDto GetHadcoShopOverhead();
    }
}
