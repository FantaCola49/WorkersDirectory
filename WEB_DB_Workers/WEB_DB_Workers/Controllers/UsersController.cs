using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_DB_Workers.Domain;
using WEB_DB_Workers.Models;

namespace WEB_DB_Workers.Controllers
{
    public class UsersController : Controller
    {
        public int pageSize = 10;
    public int showPages = 15;
    public int count = 0;
 
    // отображение списка пользователей
    public ViewResult Index(string sortOrder, int page = 1)
    {
        string sortName = null;
        System.Web.Helpers.SortDirection sortDir = System.Web.Helpers.SortDirection.Ascending;
        UsersRepository rep = new UsersRepository();
        UsersGrid users = new UsersGrid {
            Users = rep.List(sortName, sortDir, page, pageSize, out count),
            PagingInfo = new PagingInfo
            {
                currentPage = page,
                itemsPerPage = pageSize,
                totalItems = count,
                showPages = showPages
            },
            SortingInfo = new SortingInfo {
                currentOrder = sortName,
                currentDirection = sortDir
            }
        };
        return View(users);
    }
 
    [HttpPost]
    public ActionResult Index(string onNewUser)
    {
        if (onNewUser != null) {
            TempData["referrer"] = ControllerContext.RouteData.Values["referrer"];
            return View("New", new UserModel(new UserClass(), Languages()));
        }
        return View();
    }
 
    public ActionResult New()
    {
        TempData["referrer"] = ControllerContext.RouteData.Values["referrer"];
        return View("New", new UserModel(new UserClass(), Languages()));
    }
 
    [HttpPost]
    public ActionResult New(UserModel model)
    {
        if (ModelState.IsValid)
        {
            if (model.User == null || model.User.Language == null || model.User.Language.LanguageID == 0) RedirectToAction("Index");
            UsersRepository rep = new UsersRepository();
            if (rep.AddUser(model.User)) TempData["message"] = string.Format("{0} has been added", model.User.Loginname);
            else TempData["error"] = string.Format("{0} has not been added!", model.User.Loginname);
            if (TempData["referrer"] != null) return Redirect(TempData["referrer"].ToString());
            return RedirectToAction("Index");
        }
        else
        {
            model = new UserModel(model.User, Languages()); // почему-то при невалидной модели в данный метод приходит пустой список model.Languages, приходится перезаполнять
            return View(model);
        }
    }
 
    public ActionResult Edit(int UserID)
    {
        UsersRepository rep = new UsersRepository();
        UserClass user = rep.FetchByID(UserID);
        if (user == null) return HttpNotFound();
        TempData["referrer"] = ControllerContext.RouteData.Values["referrer"];
        return View(new UserModel(user, Languages()));
    }
 
    [HttpPost]
    public ActionResult Edit(UserModel model, string action)
    {
        if (action == "Cancel")
        {
            if (TempData["referrer"] != null) return Redirect(TempData["referrer"].ToString());
            return RedirectToAction("Index");
        }
        if (ModelState.IsValid)
        {
            if (model.User == null || model.User.Language == null || model.User.Language.LanguageID == 0) RedirectToAction("Index");
            UsersRepository rep = new UsersRepository();
            if (action == "Save")
            {
                if (rep.ChangeUser(model.User)) TempData["message"] = string.Format("{0} has been saved", model.User.Loginname);
                else TempData["error"] = string.Format("{0} has not been saved!", model.User.Loginname);
            }
            if (action == "Remove")
            {
                if (rep.RemoveUser(model.User)) TempData["message"] = string.Format("{0} has been removed", model.User.Loginname);
                else TempData["error"] = string.Format("{0} has not been removed!", model.User.Loginname);
            }
            if (TempData["referrer"] != null) return Redirect(TempData["referrer"].ToString());
            return RedirectToAction("Index");
        }
        else
        {
            model = new UserModel(model.User, Languages()); 
            return View(model);
        }
    }
 
    public IList<LanguageClass> Languages()
    {
        IList<LanguageClass> languages = new List<LanguageClass>();
        LanguagesRepository rep = new LanguagesRepository();
        languages = rep.List();
        return languages;
    }
    }
}