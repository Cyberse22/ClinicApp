using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClinicWeb.App_Data;

namespace ClinicWeb.Models
{
    public class PrescriptionViewModel
    {
        [Display(Name = "ID Cuộc hẹn")]
        public int AppointmentId { get; set; }

        [Display(Name = "Ngày Cuộc hẹn")]
        public DateTime AppointmentDate { get; set; }

        [Display(Name = "Giờ Cuộc hẹn")]
        public TimeSpan AppointmentTime { get; set; }

        [Display(Name = "ID Bệnh nhân")]
        public int PatientId { get; set; }

        [Display(Name = "Tên Bệnh nhân")]
        public string PatientName { get; set; }

        [Display(Name = "ID Bác sĩ")]
        public int DoctorId { get; set; }

        [Display(Name = "Tên Bác sĩ")]
        public string DoctorName { get; set; }

        [Display(Name = "Kết luận")]
        public string Conclusion { get; set; }

        [Display(Name = "Danh sách Thuốc")]
        public List<App_Data.Medicine> Medicines { get; set; }

        public List<PrescriptionDetailViewModel> PrescriptionDetails { get; set; }

        public PrescriptionViewModel()
        {
            PrescriptionDetails = new List<PrescriptionDetailViewModel>();
        }
    }

    public class PrescriptionDetailViewModel
    {
        [Display(Name = "Thuốc")]
        public int MedicineId { get; set; }

        [Display(Name = "Tên Thuốc")]
        public string MedicineName { get; set; }

        [Display(Name = "Số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng nhập giá trị lớn hơn {1}")]
        public int Quantity { get; set; }
    }
}
