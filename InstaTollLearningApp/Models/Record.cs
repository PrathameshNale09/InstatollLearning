using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstaTollLearningApp.Models
{
    internal class Record
    {
        public int RecordId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
