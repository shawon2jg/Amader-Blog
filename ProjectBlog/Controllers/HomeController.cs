using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectBlog.Models;

namespace ProjectBlog.Controllers
{
    public class HomeController : Controller
    {
        ProjectBlogDB aProjectBlogDb = new ProjectBlogDB();
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                var userId = Convert.ToInt32(Session["UserID"]);
                ViewBag.User = aProjectBlogDb.Users.Find(userId);
            }
            var mostViewed = aProjectBlogDb.Posts.Where(d=>d.IsPublished).OrderByDescending(d => d.NoOfView).Take(5).ToList();
            var recent = aProjectBlogDb.Posts.Where(d => d.IsPublished).OrderByDescending(d => d.PostID).Take(5).ToList();
            foreach (Post post in mostViewed)
            {
                post.Content = post.Content.Take(200)
                    .Aggregate("", (current, ch) => current + ch);
            }
            foreach (Post post in recent)
            {
                post.Content = post.Content.Take(200)
                    .Aggregate("", (current, ch) => current + ch);
            }
            ViewBag.MostViewed = mostViewed;
            ViewBag.Recent = recent;
            ViewBag.Authors = aProjectBlogDb.Users;
            return View();
        }

        public ActionResult SignUp()
        {
            if (Session["UserID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(User anUser)
        {
            if (ModelState.IsValid)
            {
                var user = aProjectBlogDb.Users.FirstOrDefault(a => a.Email.Equals(anUser.Email));
                if (user == null)
                {
                    anUser.CreatedDate = DateTime.Now;
                    aProjectBlogDb.Users.Add(anUser);
                    aProjectBlogDb.SaveChanges();
                    Session["UserID"] = anUser.UserID.ToString();
                    Session["Email"] = anUser.Email;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Message = "An account with this email already exists";
                }
            }
            else
            {
                ViewBag.Message = "Please fill-up the form correctly";
            }
            return View();
        }

        public ActionResult SignIn()
        {
            if (Session["UserID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(User user)
        {
            var findUser = aProjectBlogDb.Users.FirstOrDefault(a => a.Email.Equals(user.Email) && a.Password.Equals(user.Password));
            if (findUser != null)
            {
                Session["UserID"] = findUser.UserID.ToString();
                Session["Email"] = findUser.Email;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "The Email & Password you entered is incorrect. Please try again.";
            }

            return View(user);
        }

        public ActionResult SignOut()
        {
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult About()
        {
            if (Session["UserID"] != null)
            {
                var userId = Convert.ToInt32(Session["UserID"]);
                ViewBag.User = aProjectBlogDb.Users.Find(userId);
            }
            return View();
        }

        public ActionResult AllPosts(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (Session["UserID"] != null)
            {
                var userId = Convert.ToInt32(Session["UserID"]);
                ViewBag.User = aProjectBlogDb.Users.Find(userId);
            }
            ViewBag.NameOfAuthor = aProjectBlogDb.Users.Find(id).UserName;
            var posts = aProjectBlogDb.Posts.Where(x => (x.UserID == id) && x.IsPublished).ToList();

            foreach (Post post in posts)
            {
                post.Content = post.Content.Take(200).Aggregate("", (current, ch) => current + ch);
            }
            ViewBag.Authors = aProjectBlogDb.Users;
            return View(posts);
        }
    }
}