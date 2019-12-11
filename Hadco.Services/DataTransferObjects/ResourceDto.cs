using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Hadco.Services.DataTransferObjects
{
	public class ResourceDto : IDataTransferObject
	{
		[JsonProperty(PropertyName = "ResourceID")]
		public int ID { get; set; }
		[Required]
		public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ResourceDto))
                return false;

            var that = obj as ResourceDto;

            return this.ID.Equals(that.ID);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }
	}
}
