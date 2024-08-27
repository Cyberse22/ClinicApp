using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ClinicWeb.Models
{
    public class AppointmentPatientViewModel
    {
        public int Id { get; set; }

        public int PatientId { get; set; }
        public string PatientLastName { get; set; }
        public string PatientFirstName { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public System.TimeSpan Time { get; set; }

        public string Reason { get; set; }
    }
}