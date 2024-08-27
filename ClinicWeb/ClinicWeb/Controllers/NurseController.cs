using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClinicWeb.App_Data;
using ClinicWeb.Models;

namespace ClinicWeb.Controllers
{
    public class NurseController : Controller
    {

        private QLPKEntities db = new QLPKEntities();
        // GET: Nurse
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult AddAppointment()
        {
            var appointmentViewModel = new AppointmenViewModel();
            var doctors = db.Doctor.ToList();
            ViewBag.Doctors = doctors;
            return View(appointmentViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAppointment(AppointmenViewModel appointmenViewModel)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var nurse = (Nurse)Session["user_details"];
                    var appointment = new Appointment
                    {
                        Patient = new Patient
                        {
                            UserInfo = new UserInfo
                            {
                                last_name = appointmenViewModel.LastName,
                                first_name = appointmenViewModel.FirstName,
                                gender = appointmenViewModel.Gender,
                                date_of_birth = appointmenViewModel.dob,
                                address = appointmenViewModel.Address,
                                phone = appointmenViewModel.Phone,
                                active = true
                            }
                        },
                        doctor_id = appointmenViewModel.DoctorId,
                        nurse_id = nurse.id,
                        date = appointmenViewModel.date,
                        time = appointmenViewModel.time,
                        reason = appointmenViewModel.reason,
                        status = "scheduled"
                    };

                    db.Appointment.Add(appointment);
                    db.SaveChanges();

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo lịch khám. Vui lòng thử lại sau.");
                }
            }

            return View();
        }

        public ActionResult GetListAppointment()
        {
            //outer join 
            var query1 = from appointment in db.Appointment
                         join patient in db.Patient on appointment.patient_id equals patient.id
                         join info in db.UserInfo on patient.info_id equals info.id
                         join invoice in db.Invoice on appointment.id equals invoice.appointment_id into appointmentGroup
                         from invoice in appointmentGroup.DefaultIfEmpty()
                         where appointment.status.Contains("completed")
                         select new
                         {
                             Id = appointment.id,
                             PatientId = patient.id,
                             Patient_LastName = info.last_name,
                             Patient_FirstName = info.first_name,
                             Date = appointment.date,
                             Time = appointment.time,
                             Reason = appointment.reason,
                         };
            //inner join
            var query2 = from appointment in db.Appointment
                         join patient in db.Patient on appointment.patient_id equals patient.id
                         join info in db.UserInfo on patient.info_id equals info.id
                         join invoice in db.Invoice on appointment.id equals invoice.appointment_id
                         where appointment.status.Contains("completed")
                         select new
                         {
                             Id = appointment.id,
                             PatientId = patient.id,
                             Patient_LastName = info.last_name,
                             Patient_FirstName = info.first_name,
                             Date = appointment.date,
                             Time = appointment.time,
                             Reason = appointment.reason,
                         };

            var outerJoinList = query1.ToList();
            var innerJoinList = query2.ToList();

            var result = outerJoinList.Except(innerJoinList).ToList();

            var appointmentPatientViewModel = result.Select(item => new AppointmentPatientViewModel
            {
                Id = item.Id,
                PatientId = item.PatientId,
                PatientLastName = item.Patient_LastName,
                PatientFirstName = item.Patient_FirstName,
                Date = item.Date,
                Time = item.Time,
                Reason = item.Reason
            }).ToList();


            return View(appointmentPatientViewModel);
        }

        public ActionResult CreateInvoice(int id)
        {
            //lấy ra prescription có appointmentId = id, join với prescriptiondetail và medicine => lấy ra quantity và medicineName
            var query1 = (from prescription in db.Prescription
                          join patient in db.Patient on prescription.patient_id equals patient.id
                          join info in db.UserInfo on patient.info_id equals info.id
                          where prescription.appointment_id == id
                          select new
                          {
                              Id = prescription.id,
                              PatientId = prescription.patient_id,
                              AppointmentId = prescription.appointment_id,
                              FirstName = info.first_name,
                              LastName = info.last_name,

                          }).FirstOrDefault();
            var query2 = (from prescription in db.Prescription
                          join pres_detail in db.Prescription_Detail on prescription.id equals pres_detail.prescription_id
                          join medicine in db.Medicine on pres_detail.medicine_id equals medicine.id
                          where prescription.appointment_id == id
                          select new
                          {
                              MedicineName = medicine.name,
                              Price = medicine.unit_price,
                              Quantity = pres_detail.quantity,

                          }).ToList();


            if (query1 != null)
            {
                var invoiceViewModel = new InvoiceViewModel
                {
                    Id = query1.Id,
                    PatientId = query1.PatientId,
                    AppointmentId = query1.AppointmentId,
                    PatientFirstName = query1.FirstName,
                    PatientLastName = query1.LastName,
                    Medicines = query2.Select(m => new Models.Medicine
                    {
                        MedicineName = m.MedicineName,
                        Price = (decimal)m.Price,
                        Quantity = (int)m.Quantity
                    }).ToList(),
                    ServiceId = 0,
                    ServiceQuantity = 0

                };

                var services = db.Service.ToList();
                ViewBag.Service = services;

                return View(invoiceViewModel);
            }
            return View();


        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateInvoice(InvoiceViewModel invoiceViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var nurse = (Nurse)Session["user_details"];
                    var service = (from ser in db.Service
                                   where ser.id == invoiceViewModel.ServiceId
                                   select new
                                   {
                                       UnitPrice = ser.unit_price,
                                       Name = ser.name
                                   }).First();

                    invoiceViewModel.ServicePrice = (decimal)(invoiceViewModel.ServiceQuantity * service.UnitPrice);
                    invoiceViewModel.TotalAmount = invoiceViewModel.ServicePrice + invoiceViewModel.MedicinePrice;
                    invoiceViewModel.ServiceName = service.Name;

                    var InvoiceService = new Invoice_Service
                    {

                        service_id = invoiceViewModel.ServiceId,
                        quantity = invoiceViewModel.ServiceQuantity,
                        unit_price = service.UnitPrice,

                        Invoice = new Invoice
                        {
                            appointment_id = invoiceViewModel.AppointmentId,
                            prescription_id = invoiceViewModel.Id,
                            patient_id = invoiceViewModel.PatientId,
                            nurse_id = nurse.id,
                            total_amount = invoiceViewModel.TotalAmount
                        },
                    };


                    db.Invoice_Service.Add(InvoiceService);
                    db.SaveChanges();

                    return RedirectToAction("ExportInvoice", new { InvoiceService.Invoice.id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi xuất hóa đơn. Vui lòng thử lại sau.");
                }
            }
            return View();
        }

        public ActionResult ExportInvoice(int id)
        {
            var query1 = (from invoice in db.Invoice
                          join patient in db.Patient on invoice.patient_id equals patient.id
                          join info in db.UserInfo on patient.info_id equals info.id
                          join invoice_service in db.Invoice_Service on invoice.id equals invoice_service.invoice_id
                          join service in db.Service on invoice_service.service_id equals service.id
                          where invoice.id == id
                          select new
                          {
                              Id = invoice.id,
                              PatientId = invoice.patient_id,
                              FirstName = info.first_name,
                              LastName = info.last_name,
                              ServiceName = service.name,
                              ServicePrice = service.unit_price,
                              ServiceQuantity = invoice_service.quantity,
                              TotalAmount = invoice.total_amount

                          }).FirstOrDefault();
            var query2 = (from invoice in db.Invoice
                          join pres_detail in db.Prescription_Detail on invoice.prescription_id equals pres_detail.prescription_id
                          join medicine in db.Medicine on pres_detail.medicine_id equals medicine.id
                          where invoice.id == id
                          select new
                          {
                              MedicineName = medicine.name,
                              Price = medicine.unit_price,
                              Quantity = pres_detail.quantity,

                          }).ToList();

            var invoiceViewModel = new InvoiceViewModel
            {

                Id = query1.Id, //id là invoice id 
                PatientId = query1.PatientId,

                PatientFirstName = query1.FirstName,
                PatientLastName = query1.LastName,
                Medicines = query2.Select(m => new Models.Medicine
                {
                    MedicineName = m.MedicineName,
                    Price = (decimal)m.Price,
                    Quantity = (int)m.Quantity
                }).ToList(),
                ServiceName = query1.ServiceName,
                ServicePrice = (decimal)query1.ServicePrice,
                ServiceQuantity = (int)query1.ServiceQuantity,
                TotalAmount = (decimal)query1.TotalAmount
            };


            return View(invoiceViewModel);
        }
    }
}