using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClinicWeb.App_Data;

namespace ClinicWeb.Controllers
{
    public class HomeController : Controller
    {
        private QLPKEntities db = new QLPKEntities();

        public ActionResult Index()
        {
            if (Session["user_details"]  != null)
            {
                switch (Session["user_role"].ToString())
                {
                    case "admin":
                        Admin a = (Admin)Session["user_details"];
                        Session["firstname"] = a.UserInfo.first_name;
                        break;
                    case "doctor":
                        Doctor d = (Doctor)Session["user_details"];
                        Session["firstname"] = d.UserInfo.first_name;
                        break;
                    case "nurse":
                        Nurse n = (Nurse)Session["user_details"];
                        Session["firstname"] = n.UserInfo.first_name;
                        break;
                    case "patient":
                        Patient p = (Patient)Session["user_details"];
                        Session["firstname"] = p.UserInfo.first_name;
                        break;
                }
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}