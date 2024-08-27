using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Text;
using ClinicWeb.App_Data;
using ClinicWeb.Models;

namespace ClinicWeb.Controllers
{
    public class AdminController : Controller
    {
        private QLPKEntities db = new QLPKEntities();

        // GET: Admin
        public ActionResult Index()
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Admin/Doctors
        public ActionResult Doctors(string kw)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            if (!string.IsNullOrEmpty(kw))
            {
                var d = db.Doctor.Where(a => a.UserInfo.first_name.Contains(kw) || a.UserInfo.last_name.Contains(kw)).ToList();
                return View(d);
            }

            var doctors = db.Doctor.ToList();
            return View(doctors);
        }

        // GET: Admin/EditDoctor/id
        public ActionResult EditDoctor(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var doctor = db.Doctor.Include("UserInfo").FirstOrDefault(d => d.id == id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDoctor(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                var existingDoctor = db.Doctor.Include("UserInfo").FirstOrDefault(d => d.id == doctor.id);
                if (existingDoctor != null)
                {
                    existingDoctor.UserInfo.last_name = doctor.UserInfo.last_name;
                    existingDoctor.UserInfo.first_name = doctor.UserInfo.first_name;
                    existingDoctor.UserInfo.gender = doctor.UserInfo.gender;
                    existingDoctor.UserInfo.date_of_birth = doctor.UserInfo.date_of_birth;
                    existingDoctor.UserInfo.address = doctor.UserInfo.address;
                    existingDoctor.UserInfo.phone = doctor.UserInfo.phone;

                    existingDoctor.UserInfo.Account.username = doctor.UserInfo.Account.username;
                    existingDoctor.UserInfo.Account.password = HashPassword(doctor.UserInfo.Account.password);

                    existingDoctor.specialty = doctor.specialty;

                    db.SaveChanges();
                    return RedirectToAction("Doctors");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return View(doctor);
        }

        // GET: Admin/AddDoctor
        public ActionResult AddDoctor()
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var doctorViewModel = new DoctorViewModel();

            return View(doctorViewModel);
        }

        // POST: Admin/AddDoctor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDoctor(DoctorViewModel doctorViewModel)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var doctor = new Doctor
                    {
                        UserInfo = new UserInfo
                        {
                            first_name = doctorViewModel.FirstName,
                            last_name = doctorViewModel.LastName,
                            gender = doctorViewModel.Gender,
                            date_of_birth = doctorViewModel.DateOfBirth,
                            address = doctorViewModel.Address,
                            phone = doctorViewModel.Phone,
                            Account = new Account
                            {
                                username = doctorViewModel.Username,
                                password = HashPassword(doctorViewModel.Username),
                                avatar = "https://www.w3schools.com/bootstrap5/img_avatar1.png",
                                user_role = "doctor"
                            },
                        },
                        specialty = doctorViewModel.Specialty
                    };

                    db.Doctor.Add(doctor);
                    db.SaveChanges();

                    return RedirectToAction("Doctors", "Admin");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm bác sĩ. Vui lòng thử lại sau.");
                }
            }

            return View(doctorViewModel);
        }

        // GET: Admin/DeleteDoctor/5
        public ActionResult DeleteDoctor(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var doctor = db.Doctor.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }

            return View(doctor);
        }

        // POST: Admin/DeleteDoctor/5
        [HttpPost, ActionName("DeleteDoctor")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteDoctorConfirmed(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            try
            {
                var doctor = db.Doctor.Find(id);
                if (doctor == null)
                {
                    return HttpNotFound();
                }

                var appointments = db.Appointment.Where(a => a.doctor_id == id).ToList();
                if (appointments.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa bác sĩ vì bác sĩ đang có cuộc hẹn.";
                    return RedirectToAction("Doctors");
                }

                var prescriptions = db.Prescription.Where(p => p.doctor_id == id).ToList();
                if (prescriptions.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa bác sĩ vì bác sĩ đang có toa thuốc.";
                    return RedirectToAction("Doctors");
                }

                db.Account.Remove(doctor.UserInfo.Account);
                db.UserInfo.Remove(doctor.UserInfo);
                db.Doctor.Remove(doctor);
                db.SaveChanges();

                return RedirectToAction("Doctors");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi xóa bác sĩ. Vui lòng thử lại sau.");
                return View();
            }
        }

        // GET: Admin/Nurses
        public ActionResult Nurses(string kw)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            if (!string.IsNullOrEmpty(kw))
            {
                var d = db.Nurse.Where(a => a.UserInfo.first_name.Contains(kw) || a.UserInfo.last_name.Contains(kw)).ToList();
                return View(d);
            }

            var nurses = db.Nurse.ToList();

            return View(nurses);
        }

        // GET: Admin/AddNurse
        public ActionResult AddNurse()
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var nurseViewModel = new NurseViewModel();

            return View(nurseViewModel);
        }

        // POST: Admin/AddNurse
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddNurse(NurseViewModel nurseViewModel)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var nurse = new Nurse
                    {
                        UserInfo = new UserInfo
                        {
                            first_name = nurseViewModel.FirstName,
                            last_name = nurseViewModel.LastName,
                            gender = nurseViewModel.Gender,
                            date_of_birth = nurseViewModel.DateOfBirth,
                            address = nurseViewModel.Address,
                            phone = nurseViewModel.Phone,
                            Account = new Account
                            {
                                username = nurseViewModel.Username,
                                password = HashPassword(nurseViewModel.Username),
                                avatar = "https://www.w3schools.com/bootstrap5/img_avatar1.png",
                                user_role = "nurse"
                            },
                        }
                    };

                    db.Nurse.Add(nurse);
                    db.SaveChanges();

                    return RedirectToAction("Nurses", "Admin");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm y tá. Vui lòng thử lại sau.");
                }
            }

            return View(nurseViewModel);
        }

        // GET: Admin/EditNurse/id
        public ActionResult EditNurse(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var nurse = db.Nurse.Include("UserInfo").FirstOrDefault(d => d.id == id);
            if (nurse == null)
            {
                return HttpNotFound();
            }
            return View(nurse);
        }

        // POST: Admin/EditNurse/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditNurse(Nurse nurse)
        {
            if (ModelState.IsValid)
            {
                var existingNurse = db.Nurse.Include("UserInfo").FirstOrDefault(d => d.id == nurse.id);
                if (existingNurse != null)
                {
                    existingNurse.UserInfo.last_name = nurse.UserInfo.last_name;
                    existingNurse.UserInfo.first_name = nurse.UserInfo.first_name;
                    existingNurse.UserInfo.gender = nurse.UserInfo.gender;
                    existingNurse.UserInfo.date_of_birth = nurse.UserInfo.date_of_birth;
                    existingNurse.UserInfo.address = nurse.UserInfo.address;
                    existingNurse.UserInfo.phone = nurse.UserInfo.phone;

                    existingNurse.UserInfo.Account.username = nurse.UserInfo.Account.username;
                    existingNurse.UserInfo.Account.password = HashPassword(nurse.UserInfo.Account.password);

                    db.SaveChanges();
                    return RedirectToAction("Nurses");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return View(nurse);
        }

        // GET: Admin/DeleteNurse/5
        public ActionResult DeleteNurse(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var nurse = db.Nurse.Find(id);
            if (nurse == null)
            {
                return HttpNotFound();
            }

            return View(nurse);
        }

        // POST: Admin/DeleteNurse/5
        [HttpPost, ActionName("DeleteNurse")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteNurseConfirmed(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            try
            {
                var nurse = db.Nurse.Find(id);
                if (nurse == null)
                {
                    return HttpNotFound();
                }

                var appointments = db.Appointment.Where(a => a.nurse_id == id).ToList();
                if (appointments.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa y tá vì y tá đang có cuộc hẹn.";
                    return RedirectToAction("Nurses");
                }

                var invoices = db.Invoice.Where(a => a.nurse_id == id).ToList();
                if (invoices.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa y tá vì y tá đẫ có trong hóa đơn.";
                    return RedirectToAction("Nurses");
                }

                db.Account.Remove(nurse.UserInfo.Account);
                db.UserInfo.Remove(nurse.UserInfo);
                db.Nurse.Remove(nurse);
                db.SaveChanges();

                return RedirectToAction("Nurses");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi xóa y tá. Vui lòng thử lại sau.");
                return View();
            }
        }

        // GET: Admin/Medicines
        public ActionResult Medicines(string kw)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            if (!string.IsNullOrEmpty(kw))
            {
                var d = db.Medicine.Where(a => a.name.Contains(kw)).ToList();
                return View(d);
            }

            var medicines = db.Medicine.ToList();

            return View(medicines);
        }

        // GET: Admin/AddMedicine
        public ActionResult AddMedicine()
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var medicineViewModel = new MedicineViewModel();

            return View(medicineViewModel);
        }

        // POST: Admin/AddMedicine
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMedicine(MedicineViewModel medicineViewModel)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var medicine = new App_Data.Medicine
                    {
                        name = medicineViewModel.Name,
                        unit_price = medicineViewModel.UnitPrice
                    };

                    db.Medicine.Add(medicine);
                    db.SaveChanges();

                    return RedirectToAction("Medicines", "Admin");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi thêm thuốc. Vui lòng thử lại sau.");
                }
            }

            return View(medicineViewModel);
        }

        // GET: Admin/EditMedicine/id
        public ActionResult EditMedicine(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var medicine = db.Medicine.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }
            return View(medicine);
        }

        // POST: Admin/EditMedicine/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMedicine(App_Data.Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                var existingMedicine = db.Medicine.Find(medicine.id);
                if (existingMedicine != null)
                {
                    existingMedicine.name = medicine.name;
                    existingMedicine.unit_price = medicine.unit_price;

                    db.SaveChanges();
                    return RedirectToAction("Medicines");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return View(medicine);
        }

        // GET: Admin/DeleteMedicine/5
        public ActionResult DeleteMedicine(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var medicine = db.Medicine.Find(id);
            if (medicine == null)
            {
                return HttpNotFound();
            }

            return View(medicine);
        }

        // POST: Admin/DeleteMedicine/5
        [HttpPost, ActionName("DeleteMedicine")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMedicineConfirmed(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            try
            {
                var medicine = db.Medicine.Find(id);
                if (medicine == null)
                {
                    return HttpNotFound();
                }

                var prescriptions = db.Prescription_Detail.Where(p => p.medicine_id == id).ToList();
                if (prescriptions.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa thuốc vì thuốc đang được sử dụng trong toa thuốc.";
                    return RedirectToAction("Medicines");
                }

                db.Medicine.Remove(medicine);
                db.SaveChanges();

                return RedirectToAction("Medicines");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi xóa thuốc. Vui lòng thử lại sau.");
                return View();
            }
        }

        // GET: Admin/Services
        public ActionResult Services(string kw)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            if (!string.IsNullOrEmpty(kw))
            {
                var d = db.Service.Where(a => a.name.Contains(kw)).ToList();
                return View(d);
            }

            var services = db.Service.ToList();
            return View(services);
        }

        // GET: Admin/AddService
        public ActionResult AddService()
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var serviceViewModel = new ServiceViewModel();
            return View(serviceViewModel);
        }

        // POST: Admin/AddService
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddService(ServiceViewModel serviceViewModel)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            if (ModelState.IsValid)
            {
                var service = new Service
                {
                    name = serviceViewModel.Name,
                    unit_price = serviceViewModel.UnitPrice
                };

                db.Service.Add(service);
                db.SaveChanges();

                return RedirectToAction("Services");
            }
            return View(serviceViewModel);
        }

        // GET: Admin/EditService/id
        public ActionResult EditService(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var service = db.Service.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }

            return View(service);
        }

        // POST: Admin/EditService/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditService(Service service)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            if (ModelState.IsValid)
            {
                var existingService = db.Service.Find(service.id);
                if (existingService != null)
                {
                    existingService.name = service.name;
                    existingService.unit_price = service.unit_price;

                    db.SaveChanges();
                    return RedirectToAction("Services");
                }
                else
                {
                    return HttpNotFound();
                }
            }
            return View(service);
        }

        // GET: Admin/DeleteService/id
        public ActionResult DeleteService(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var service = db.Service.Find(id);
            if (service == null)
            {
                return HttpNotFound();
            }

            return View(service);
        }

        // POST: Admin/DeleteService/id
        [HttpPost, ActionName("DeleteService")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteServiceConfirmed(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "admin")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            try
            {
                var service = db.Service.Find(id);
                if (service == null)
                {
                    return HttpNotFound();
                }

                // Kiểm tra ràng buộc trước khi xóa
                var invoices = db.Invoice_Service.Where(i => i.service_id == id).ToList();
                if (invoices.Any())
                {
                    TempData["ErrorMessage"] = "Không thể xóa dịch vụ vì dịch vụ đang được sử dụng trong hóa đơn.";
                    return RedirectToAction("Services");
                }

                db.Service.Remove(service);
                db.SaveChanges();

                return RedirectToAction("Services");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi xóa dịch vụ. Vui lòng thử lại sau.");
                return View();
            }
        }

        public ActionResult Stats(string period = "MONTH", int year = 2024)
        {
            ViewBag.Year = year;
            ViewBag.Period = period;

            List<StatsViewModel> stats;

            if (period.ToUpper() == "MONTH")
            {
                stats = GetMonthlyStats(year);
            }
            else
            {
                stats = GetQuarterlyStats(year);
            }

            return View(stats);
        }

        private List<StatsViewModel> GetMonthlyStats(int year)
        {
            var invoices = db.Invoice
                .Where(a => a.created_date.HasValue && a.created_date.Value.Year == year)
                .ToList(); // Fetch data into memory

            var monthlyStats = invoices
                .GroupBy(a => a.created_date.Value.Month)
                .Select(g => new StatsViewModel
                {
                    TimePeriod = "Tháng " + g.Key,
                    Revenue = g.Sum(a => a.total_amount ?? 0)
                })
                .ToList();

            return monthlyStats;
        }

        private List<StatsViewModel> GetQuarterlyStats(int year)
        {
            var invoices = db.Invoice
                .Where(a => a.created_date.HasValue && a.created_date.Value.Year == year)
                .ToList(); // Fetch data into memory

            var quarterlyStats = invoices
                .GroupBy(a => (a.created_date.Value.Month - 1) / 3 + 1)
                .Select(g => new StatsViewModel
                {
                    TimePeriod = "Quý " + g.Key,
                    Revenue = g.Sum(a => a.total_amount ?? 0)
                })
                .ToList();

            return quarterlyStats;
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}