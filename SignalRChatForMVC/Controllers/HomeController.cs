using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SignalRChatForMVC.Model;
namespace SignalRChatForMVC.Controllers
{
    public class HomeController : Controller
    {
        public static List<UserDetail> UserBase = new List<UserDetail>();
        static HomeController()
        {
            UserBase.Add(new UserDetail { UserID = "1", UserName = "zzl", Password = "zzl" });
            UserBase.Add(new UserDetail { UserID = "2", UserName = "zhz", Password = "zhz" });
        }
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            Session["userID"] = null;
            return RedirectToAction("Login");
        }

        [HttpPost]
        public ActionResult Login(FormCollection form)
        {
            var entity = UserBase.FirstOrDefault(i => i.UserName == form["userName"]
                && i.Password == form["password"]);
            if (entity != null)
            {
                Session["userID"] = entity.UserID;
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "用户名密码不正确...");
            return View();
        }
    }
}
