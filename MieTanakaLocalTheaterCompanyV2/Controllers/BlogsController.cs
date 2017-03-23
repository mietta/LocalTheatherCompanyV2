using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MieTanakaLocalTheaterCompanyV2.Models;
using Microsoft.AspNet.Identity;
using PagedList;
/// <summary>
/// Name:Mie Tanaka
/// Name:02/03/2017
/// Description: allows users to create, edit and delete blogs
/// 

 

namespace MieTanakaLocalTheaterCompanyV2.Controllers
{
    public class BlogsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Blogs
        /*     public ActionResult Index()
             {
                 var blogs = db.Blogs.Include(b => b.Category);
                 return View(blogs.ToList());
             }
        */
        /// <summary>
        /// Returns a collection of blogs within page list
        /// With filter function by category name or input string on blog title
        /// With sort function by forename or date in ascending alternatively descending order
        /// </summary>
        /// <param name="categoryChoice"></param>
        /// <param name="searchString"></param>
        /// <param name="sortOrder"></param>
        /// <param name="currentFilter"></param>
        /// <param name="page"></param>
        /// <returns>Returns a collection of blogs within page list</returns>
        [AllowAnonymous]
        public ActionResult Index(string categoryChoice, string searchString, string sortOrder, string currentFilter, int? page)
        {
            //return all the record in blogs including cagetory if admin          
                var blogs = db.Blogs.Include(b => b.Category);

            // filter out the blog records that is not approved anyone other than Administrator or staff logged in
            if (!User.IsInRole("Administrator")&& !User.IsInRole("Staff"))
            {              
                 blogs = db.Blogs.Include(b => b.Category).Where(b => b.BlogApproved == true);

                if (!String.IsNullOrEmpty(searchString)) //if string is not null
                {   //return record contains search string
                    blogs = blogs.Where(b => b.BlogTitle.Contains(searchString) && b.BlogApproved == true);
                }
                if (!String.IsNullOrEmpty(categoryChoice))//if categorychoice is not null
                {   //return record contains the category name
                    blogs = blogs.Where(b => b.Category.CategoryName == categoryChoice && b.BlogApproved == true);
                }
                if (!String.IsNullOrEmpty(sortOrder))//if categorychoice is not null
                {   //return record contains the category name
                    blogs = db.Blogs.Include(b => b.Category).Where(b => b.BlogApproved == true);
                }
            }
            // filter out the all record depending on the search string
            if (!String.IsNullOrEmpty(searchString)) //if string is not null
            {   //return record contains search string
                 blogs = blogs.Where(b => b.BlogTitle.Contains(searchString));
            }

            //create list that will be shown in dropdownlist
            var categoryLst = new List<string>();
            //make categories a-z order
            var categoryQry = from d in db.Categories
                              orderby d.CategoryName
                              select d.CategoryName;
            //add distinct list of category
            categoryLst.AddRange(categoryQry.Distinct());
            //pass the sorted category list to the view
            ViewBag.categoryChoice = new SelectList(categoryLst);
            // filter out the all record depending on the categoryChoice selection
            if (!String.IsNullOrEmpty(categoryChoice))//if categorychoice is not null
            {   //return record contains the category name
                blogs = blogs.Where(b => b.Category.CategoryName == categoryChoice);
            }
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            switch (sortOrder)
                {
                    case "name_desc":
                        blogs = blogs.OrderByDescending(b => b.ApplicationUser.Forename);
                        break;
                    case "Date":
                        blogs = blogs.OrderBy(b => b.BlogDate);
                        break;
                    case "date_desc":
                        blogs = blogs.OrderByDescending(b => b.BlogDate);
                        break;
                    default:
                        blogs = blogs.OrderBy(b => b.ApplicationUser.Forename);
                        break;
                }

            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(blogs.ToPagedList(pageNumber, pageSize));


      //      return View(blogs.ToList());
        }

        // GET: Blogs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // GET: Blogs/Create
        /// <summary>
        /// returns a blank Blogs/Create view saves the input data in ot blog object
        /// </summary>
        /// <returns>Blog blog</returns>
        public ActionResult Create()
        {
            var blog = new Blog();
            blog.Id= User.Identity.GetUserId();
  //          ViewBag.Id = new SelectList(db.Users, "Id", "Forename", User.Identity.GetUserId());
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");

            return View(blog);
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// returns a Blogs/Index view after successfully save input data on to database
        /// if error return Blogs/create blog view with current blog data
        /// </summary>
        /// <param name="blog"></param>
        /// <returns> view Blogs/Index </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BlogId,BlogTitle,BlogContent,BlogDate,BlogApproved,CategoryId,Id")] Blog blog)
        { 
            blog.Id = User.Identity.GetUserId();
            
            if (ModelState.IsValid)
            {
                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

//            ViewBag.Id = new SelectList(db.Users, "Id", "Forename", blog.Id);
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", blog.CategoryId);
            return View(blog);
        }

        // GET: Blogs/Edit/5
        /// <summary>
        /// Returns Blogs/Edit view filled with the data of selected blog 
        /// and pass updated object blog to the next Blogs/Edit vew
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Blog blog</returns>
        public ActionResult Edit(int? id)
        {//Blog tempblog = TempData["tempBlog"] as Blog;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
 //           ViewBag.Id = new SelectList(db.Users, "Id", "Forename", blog.Id);
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", blog.CategoryId);
            TempData["tempBlog"] = blog;
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Returns Blogs/Index View after updating changed data on database
        /// if error keep returning the  Blogs/Edit view
        /// </summary>
        /// <param name="blog"></param>
        /// <returns> BlogViewModels/BlogViewModelIndexView </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BlogId,BlogTitle,BlogContent,BlogDate,BlogApproved,CategoryId,Id")] Blog blog)
        { Blog tempblog = TempData["tempBlog"] as Blog;
            blog.Id = tempblog.Id;
            blog.BlogDate = tempblog.BlogDate;
            if (ModelState.IsValid)
            {
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("BlogViewModelIndex","BlogViewModels",new { blogid = blog.BlogId });
            }
//            ViewBag.Id = new SelectList(db.Users, "Id", "Forename", blog.Id);
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", blog.CategoryId);
            return View(blog);
        }

        // GET: Blogs/Delete/5
        /// <summary>
        /// Blogs/Delete view filled with the selected blog with delete confirmation button
        /// and pass the blog object to next Blogs/Edit view
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Blog blog</returns>
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Delete/5
        /// <summary>
        /// Returns Blogs/Index view after successfully deleting the selected blog record
        /// </summary>
        /// <param name="id"></param>
        /// <returns> Blogs/Index </returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Blog blog = db.Blogs.Find(id);
            db.Blogs.Remove(blog);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// dispose deleted data appropriately form the database
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
