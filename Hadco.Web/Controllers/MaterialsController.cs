using Hadco.Services;
using Hadco.Services.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData;
using Hadco.Web.Models;

namespace Hadco.Web.Controllers
{
	/// <summary>
	/// 
	/// </summary>
    public class MaterialsController : DefaultController<MaterialDto>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="materialService"></param>
        public MaterialsController(IMaterialService materialService) : base(materialService)
		{ }
        
	}
}
