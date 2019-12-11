using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Services.DataTransferObjects
{
    public class PostSettingDto
    {
        public int BreadCrumbSeconds { get; set; }
    }
    public class SettingDto : PostSettingDto, IDataTransferObject
    {
        public int ID { get; set; }
        public DateTimeOffset ModifiedTime { get; set; }
    }
}
