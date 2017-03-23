using MieTanakaLocalTheaterCompanyV2.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
/// <summary>
/// Name:Mie Tanaka
/// Name:02/03/2017
/// Description: allows users to view blog and comment attached to the blog and add comment
///

/// public ActionResult AddComment(Comment comment)
/// Returns to BlogVeiwModelIndex view after passed the input data from from view gets saved it on to database
/// 

namespace MieTanakaLocalTheaterCompanyV2.Controllers
{



    public class BlogViewModelsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();



        // GET: BlogViewModel
        /// <summary>
        /// returns  a BlogViewModelIndex view with a selected blog and list of the blog comments.
        /// </summary>
        /// <param name="blogid"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult BlogViewModelIndex(int blogid, string returnUrl)
        { 
            //Recieve blog TempData from previous view
            Blog passedblog = TempData["tempBlog"] as Blog;
            // in case of entering this view by means of back button or back from addComment
            if (blogid.Equals(null))
            {
                blogid = passedblog.BlogId;
            }
            //create a blog to find the selected blog.
            Blog blog = db.Blogs.Find(blogid);
            // create mymodel that get passed the view
            // and allocate the blog value in to my model
            BlogViewModel mymodel = new BlogViewModel()
            {
                BlogTitle = blog.BlogTitle,
                BlogDate = blog.BlogDate,
                BlogContent = blog.BlogContent,
                Category = blog.Category,
                ApplicationUser = blog.ApplicationUser,
                Comments = db.Comments.Where(c => c.BlogId == blogid).ToList()
            };
            //pass chosen blog temp data to next view
            TempData["tempBlog"] = blog;
            return View(mymodel);
        }

        /// <summary>
        /// Returns a blank BlogViewModels/AddComment view and pass input cmt to next view AddComment
        /// </summary>
        /// <returns>cmt</returns>
        public ActionResult AddCommentCreate()
        {
            //Recieve blog TempData from previous view
            Blog passedblog = TempData["tempBlog"] as Blog;

            var cmt = new Comment();
            //           cmt.CommentedDate = DateTime.Now;
//            if (blogid.Equals(null)) { 
            cmt.BlogId = passedblog.BlogId;
//            }
//            cmt.BlogId = blogid;
            cmt.Id = User.Identity.GetUserId();
  //          ViewBag.BlogId = passedblog.BlogId/*new SelectList(db.Blogs, "BlogId", "BlogTitle", passedblog.BlogId)*/;
  //          ViewBag.Id = cmt.Id/*new SelectList(db.Users, "Id", "Forename",cmt.Id)*/;

            //pass chosen blog temp data to next blog
            TempData["tempBlog"] = passedblog;
            return View(cmt);
        }

        /// <summary>
        /// Returns to BlogVeiwModelIndex view 
        /// after saving the passed input data on to database
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCommentCreate([Bind(Include = "CommentId, CommentedDate, CommentTitle, CommentBody, BlogId, Id")] Comment comment)
        {
            //Receive blog TempData from previous view
            Blog passedblog = TempData["tempBlog"] as Blog;
            comment.BlogId = passedblog.BlogId;
            comment.Id = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                //pass chosen blog temp data to next blog
 //               TempData["tempBlog"] = passedblog;
                return RedirectToAction("BlogViewModelIndex", "BlogViewModels", new { blogid = passedblog.BlogId });
            }

       
 //           ViewBag.BlogId = new SelectList(db.Blogs, "BlogId", "BlogTitle", passedblog.BlogId);
 //           ViewBag.Id = new SelectList(db.Users, "Id", "forename", comment.Id);

            //pass chosen blog temp data to next blog
            TempData["tempBlog"] = passedblog;
            return View(comment);
        }

    }
}