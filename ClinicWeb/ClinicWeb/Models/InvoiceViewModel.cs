using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicWeb.Models
{
    public class Medicine
    {
        public string MedicineName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    public class InvoiceViewModel
    {
        public Nullable<int> AppointmentId { get; set; }
        public Nullable<int> PatientId { get; set; }

        public int Id { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public List<Medicine> Medicines { get; set; }
        public int ServiceId { get; set; }

        public int ServiceQuantity { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal ServicePrice { get; set; }
        public decimal MedicinePrice { get; set; }
        public string ServiceName { get; set; }
    }
}