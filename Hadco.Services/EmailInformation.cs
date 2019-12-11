using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Hadco.Services
{
    public class EmailInformation
    {
        public int UserID { get; set; }
        public string RecipientEmailAddress { get; set; }
        
        public string RecipientFirstName { get; set; }
        public string RecipientLastName { get; set; }

        
        public string FromName { get; set; }
        public string FromEmail { get; set; }

        public string Subject { get; set; }
        public string HtmlMessage { get; set; }
    }
}
