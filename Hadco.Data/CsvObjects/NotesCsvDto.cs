using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hadco.Data.CsvObjects
{
    public class NotesCsvDto : CsvDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string NoteType { get; set; }

        public string NoteTypeDescription { get; set; }

        public bool resolved { get; set; }
    }
}
