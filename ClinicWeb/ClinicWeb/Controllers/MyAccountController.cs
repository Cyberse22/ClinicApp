using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using ClinicWeb.App_Data;
using ClinicWeb.Models;
using System.Text;

namespace ClinicWeb.Controllers
{
    public class MyAccountController : Controller
    {
        private QLPKEntities db = new QLPKEntities();

        // GET: MyAccount/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(MyLoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Username == null || model.Password == null || model.Role == null)
                {
                    ModelState.AddModelError("error", "Vui lòng chọn vai trò.");
                }
                else
                {
                    var hashedPassword = HashPassword(model.Password);
                    var user = db.Account.FirstOrDefault(a => a.username == model.Username && a.password == hashedPassword && a.user_role == model.Role.ToLower());
                    if (user != null)
                    {
                        Session["account_id"] = user.id.ToString();
                        Session["username"] = user.username;
                        Session["avatar"] = user.avatar;
                        Session["user_role"] = user.user_role;

                        switch (user.user_role.ToLower())
                        {
                            case "admin":
                                var admin = db.Admin.FirstOrDefault(a => a.UserInfo.account_id == user.id);
                                Session["user_details"] = admin;
                                break;
                            case "doctor":
                                var doctor = db.Doctor.FirstOrDefault(d => d.UserInfo.account_id == user.id);
                                Session["user_details"] = doctor;
                                break;
                            case "nurse":
                                var nurse = db.Nurse.FirstOrDefault(n => n.UserInfo.account_id == user.id);
                                Session["user_details"] = nurse;
                                break;
                            case "patient":
                                var patient = db.Patient.FirstOrDefault(p => p.UserInfo.account_id == user.id);
                                Session["user_details"] = patient;
                                break;
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("error", "Tên người dùng, mật khẩu hoặc vai trò không chính xác.");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("error", "Dữ liệu không hợp lệ.");
            }
            return View(model);
        }


        // GET: MyAccount/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
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