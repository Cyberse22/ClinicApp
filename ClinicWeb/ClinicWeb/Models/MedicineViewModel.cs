using System.ComponentModel.DataAnnotations;

namespace ClinicWeb.Models
{
    public class MedicineViewModel
    {
        [Display(Name = "Tên thuốc")]
        [Required(ErrorMessage = "Vui lòng nhập tên thuốc.")]
        public string Name { get; set; }

        [Display(Name = "Đơn giá")]
        [Required(ErrorMessage = "Vui lòng nhập đơn giá.")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải là số dương.")]
        public decimal UnitPrice { get; set; }
    }
}