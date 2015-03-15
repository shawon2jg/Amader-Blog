using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProjectBlog.Models;

namespace ProjectBlog.Controllers
{
    public class PostController : Controller
    {
        ProjectBlogDB aProjectBlogDb = new ProjectBlogDB();

        public ActionResult Create()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var userId = Convert.ToInt32(Session["UserID"]);
            ViewBag.User = aProjectBlogDb.Users.Find(userId);
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(FormCollection formCollection, string save, string saveAndPublish)
        {
            var userId = Convert.ToInt32(Session["UserID"]);
            Post aPost = new Post();
            aPost.Title = formCollection["Title"];
            aPost.Content = Server.HtmlDecode(formCollection["Content"]);
            aPost.PostedDate = DateTime.Now;
            aPost.NoOfView = 0;
            aPost.UserID = userId;
            if (save != null)
            {
                aPost.IsPublished = false;
            }
            else if (saveAndPublish != null)
            {
                aPost.IsPublished = true;
            }
            aProjectBlogDb.Posts.Add(aPost);
            aProjectBlogDb.SaveChanges();

            return RedirectToAction("MyPosts", "Post");
        }

        public ActionResult Publish(int? id)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var post = aProjectBlogDb.Posts.Find(id);
            var userId = Convert.ToInt32(Session["UserID"]);
            if (userId != post.UserID)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.User = aProjectBlogDb.Users.Find(userId);

            return View(post);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Publish(FormCollection formCollection)
        {
            int postId = Convert.ToInt32(formCollection["PostID"]);
            Post aPost = aProjectBlogDb.Posts.First(x => x.PostID == postId);
            aPost.Title = formCollection["Title"];
            aPost.Content = Server.HtmlDecode(formCollection["Content"]);
            aPost.PostedDate = DateTime.Now;
            aPost.IsPublished = true;
            aProjectBlogDb.SaveChanges();

            return RedirectToAction("MyPosts", "Post");
        }

        public ActionResult MyPosts()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var userId = Convert.ToInt32(Session["UserID"]);
            var posts = aProjectBlogDb.Posts.Where(x => x.UserID == userId);
            var publishedPosts = posts.Where(x => x.IsPublished).ToList();
            for (int i = 0; i < publishedPosts.Count; i++)
            {
                publishedPosts[i].Content = publishedPosts[i].Content.Take(200)
                    .Aggregate("", (current, ch) => current + ch);
            }
            var notPublishedPosts = posts.Where(x => x.IsPublished == false).ToList();

            for (int i = 0; i < notPublishedPosts.Count; i++)
            {
                notPublishedPosts[i].Content = notPublishedPosts[i].Content.Take(200)
                    .Aggregate("", (current, ch) => current + ch);
            }
            ViewBag.NotPublishedPosts = notPublishedPosts;
            ViewBag.User = aProjectBlogDb.Users.Find(userId);
            return View(publishedPosts);
        }

        public ActionResult PostDetails(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var userId = Convert.ToInt32(Session["UserID"]);
            var post = aProjectBlogDb.Posts.Find(id);
            if (Session["UserID"] == null || post.UserID != userId)
            {
                post.NoOfView++;
                aProjectBlogDb.Entry(post).State = EntityState.Modified;
                aProjectBlogDb.SaveChanges();
            }
            ViewBag.AuthorName = aProjectBlogDb.Users.Find(post.UserID).UserName;
            ViewBag.Comments = aProjectBlogDb.Comments.Where(x => x.PostID == post.PostID).ToList();
            foreach (var comment in ViewBag.Comments)
            {
                comment.User = aProjectBlogDb.Users.Find(comment.UserID);
            }
            var morePosts = aProjectBlogDb.Posts.Where(x => x.UserID == post.UserID && x.PostID != post.PostID && x.IsPublished).OrderByDescending(x => x.PostedDate).ToList();
            for (int i = 0; i < morePosts.Count; i++)
            {
                morePosts[i].Content = morePosts[i].Content.Take(100).Aggregate("", (current, ch) => current + ch);
            }
            ViewBag.User = aProjectBlogDb.Users.Find(userId);
            ViewBag.MorePosts = morePosts;
            return View(post);
        }

        public ActionResult Search(string searchString)
        {
            if (Session["UserID"] != null)
            {
                var userId = Convert.ToInt32(Session["UserID"]);
                ViewBag.User = aProjectBlogDb.Users.Find(userId);
            }
            if (String.IsNullOrEmpty(searchString))
            {
                return RedirectToAction("Index", "Home");
            }
            var posts =
                aProjectBlogDb.Posts.Where(x => (x.Title.Contains(searchString)) || (x.Content.Contains(searchString))).ToList();

            foreach (Post post in posts)
            {
                post.Content = post.Content.Take(200).Aggregate("", (current, ch) => current + ch);
            }
            ViewBag.Authors = aProjectBlogDb.Users;
            return View(posts);
        }

        public JsonResult AddComment(Comment aComment)
        {
            aComment.CommentDate = DateTime.Now;
            aComment.IsDeleted = false;
            aProjectBlogDb.Comments.Add(aComment);
            aProjectBlogDb.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}