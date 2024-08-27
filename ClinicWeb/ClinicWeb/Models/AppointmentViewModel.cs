using ClinicWeb.App_Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicWeb.Models
{
    public class AppointmenViewModel
    {
        public int PatientId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime dob { get; set; }

        public string Address { get; set; }
        public string Phone { get; set; }

        public int DoctorId { get; set; }

        [DataType(DataType.Date)]
        public DateTime date { get; set; }

        public System.TimeSpan time { get; set; }

        public string reason { get; set; }
    }
}
