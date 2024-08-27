using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ClinicWeb.App_Data;
using ClinicWeb.Models;

namespace ClinicWeb.Controllers
{
    public class DoctorController : Controller
    {
        private QLPKEntities db = new QLPKEntities();

        // GET: Doctor
        public ActionResult Index()
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "doctor")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Doctor/Appointments
        public ActionResult Appointments(DateTime? date, string kw)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "doctor")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            if (!date.HasValue)
            {
                date = DateTime.Today;
            }

            Doctor d = (Doctor) Session["user_details"];

            if (!string.IsNullOrEmpty(kw))
            {
                var app = db.Appointment
                .Where(a => a.doctor_id == d.id &&
                            a.date.Year == date.Value.Year &&
                            a.date.Month == date.Value.Month &&
                            a.date.Day == date.Value.Day &&
                            (a.Patient.UserInfo.last_name.Contains(kw) ||
                            a.Patient.UserInfo.first_name.Contains(kw)))
                .OrderBy(a => a.date)
                .ToList();
                return View(app);
            }

            var appointments = db.Appointment
                .Where(a => a.doctor_id == d.id &&
                            a.date.Year == date.Value.Year &&
                            a.date.Month == date.Value.Month &&
                            a.date.Day == date.Value.Day)
                .OrderBy(a => a.date)
                .ToList();

            return View(appointments);
        }

        // GET: Doctor/AppointmentDetail/5
        public ActionResult AppointmentDetail(int id)
        {
            if (Session["user_role"] == null || Session["user_role"].ToString() != "doctor")
            {
                return RedirectToAction("Login", "MyAccount");
            }

            var appointment = db.Appointment
                .Include(a => a.Patient)
                .SingleOrDefault(a => a.id == id);

            if (appointment == null)
            {
                return HttpNotFound();
            }

            return View(appointment);
        }

        // GET: Doctor/PrescribeMedication
        public ActionResult PrescribeMedication(int id)
        {
            var appointment = db.Appointment.Find(id);
            if (appointment == null)
            {
                return HttpNotFound();
            }

            if (db.Prescription.Any(a => a.appointment_id == appointment.id))
            {
                return RedirectToAction("Appointments", "Doctor");
            }

            var medicines = db.Medicine.ToList();

            var viewModel = new PrescriptionViewModel
            {
                AppointmentId = id,
                AppointmentDate = appointment.date,
                AppointmentTime = appointment.time,
                PatientId = (int) appointment.patient_id,
                DoctorId = (int) appointment.doctor_id,
                PatientName = $"{appointment.Patient.UserInfo.last_name} {appointment.Patient.UserInfo.first_name}",
                Medicines = medicines
            };

            return View(viewModel);
        }

        // POST: Doctor/PrescribeMedication
        [HttpPost]
        public ActionResult PrescribeMedication(PrescriptionViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (db.Prescription.Any(a => a.appointment_id == viewModel.AppointmentId))
                {
                    return RedirectToAction("Appointments", "Doctor");
                }

                var appointment = db.Appointment.Find(viewModel.AppointmentId);
                if (appointment == null)
                {
                    return HttpNotFound();
                }

                var prescription = new Prescription
                {
                    patient_id = appointment.patient_id,
                    doctor_id = appointment.doctor_id,
                    appointment_id = viewModel.AppointmentId,
                    conclusion = viewModel.Conclusion,
                    created_date = DateTime.Now
                };

                db.Prescription.Add(prescription);
                db.SaveChanges();

                foreach (var item in viewModel.PrescriptionDetails)
                {
                    var prescriptionDetail = new Prescription_Detail
                    {
                        prescription_id = prescription.id,
                        medicine_id = item.MedicineId,
                        quantity = item.Quantity
                    };

                    db.Prescription_Detail.Add(prescriptionDetail);
                }

                appointment.status = "completed";
                db.SaveChanges();

                return RedirectToAction("Appointments", "Doctor");
            }

            return View(viewModel);
        }


        // GET: Doctor/FinishExamination/5
        public ActionResult FinishExamination(int id)
        {
            var appointment = db.Appointment.Find(id);

            if (appointment == null)
            {
                return HttpNotFound();
            }

            appointment.status = "completed";
            
            return RedirectToAction("Appointments", "Doctor");
        }
    }
}