using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SystemRezerwacjiWizyt.Models
{
    public class AddOpinionModel
    {
        [Required]
        public string Nick { get; set; }

        [Required]
        public int Rate { get; set; }

        [Required]
        public string DoctorName { get; set; }

        [Required]
        public long DoctorId { get; set; }

        public string Description { get; set; }

        public bool Creating { get; set; }
    }
}