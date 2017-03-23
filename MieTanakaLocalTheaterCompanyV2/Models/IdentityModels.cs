using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using MieTanakaLocalTheaterCompanyV2.Controllers;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.ComponentModel;

namespace MieTanakaLocalTheaterCompanyV2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //identity's id is string so it is not included, phone number is alreaded included to 
        
        public string Forename { get; set; }
        public string Surname { get; set; }     
        public string Street { get; set; }
        public string Town { get; set; }      
//        [Display(Name ="Post code")]
        public string Postcode { get; set; }
        //navigational properties

        public virtual ICollection<Comment> Comments { get; set; }
 //       public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public ApplicationUser()
        {
            Comments = new List<Comment>();
//            Roles = new List<Role>();
            Blogs = new List<Blog>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {

        // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }


    }

    public class Category
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [DisplayName("Category")]
        public int CategoryId { get; set; }
//        [StringLength(25)]
        [DisplayName("Category")]
        public string CategoryName { get; set; }

        //navigational properties
        public virtual ICollection<Blog> Blogs { get; set; }
        public Category()
        {
            Blogs = new List<Blog>();
        }

    }

    public class Comment
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int CommentId { get; set; }

        //       [DefaultValue(typeof(DateTime), "")]
        //[DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}",ApplyFormatInEditMode = true)]

        [DisplayName("Commented date")]
        [DataType(DataType.Date)]
        public DateTime CommentedDate { get; set; }
//        [Required]
//        [StringLength(100)]
        [DisplayName("Coment title")]
        public string CommentTitle { get; set; }
//        [Required]
        [DisplayName("Coment Body")]
        public string CommentBody { get; set; }

        //Navigational properties
        [DisplayName("Blog")]
        [ForeignKey("Blog")]
        public int BlogId { get; set; }
        public virtual Blog Blog { get; set; }

        [DisplayName("User")]
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public Comment()
        {
            CommentedDate = DateTime.Now.Date;
        }

    }

    public class Blog
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int BlogId { get; set; }
//        [Required]
//          [StringLength(50, ErrorMessage = "The {0} must be at least {2}, maximum 50characters long.", MinimumLength = 6)]
          [DisplayName("Blog title")]
        public string BlogTitle { get; set; }
        //        [Required]
        //        [RegularExpression(@"^(?:.*[a-z]){30,}$", ErrorMessage = "String length must be greater than or equal 30 characters.")]
        [RegularExpression(@"^.{30,}$", ErrorMessage = "Minimum 30 characters required")]
        [DisplayName("Blog content")]
        public string BlogContent { get; set; }
        //        [DisplayFormat(DataFormatString = "{0:dd-mmm-yyyy}", ApplyFormatInEditMode = true)]
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public DateTime BlogDate { get; set; }
        [DisplayName("Approved")]
        public bool BlogApproved { get; set; }

        //navigational properties
        [DisplayName("Category")]
        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        [DisplayName("User")]
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public Blog()
        {
            Comments = new List<Comment>();
            BlogDate = DateTime.Now.Date;
        }
    }

  
 /*   public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string first_name { get; set; }
        [Required]
        [StringLength(50)]
        public string last_name { get; set; }
        [Required]
        [DisplayName("User name")]
        [StringLength(25)]
        public string username { get; set; }
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        //require minimum 9 chars length, 1 special char and 1 digit and 1 Uppercase letter
        [RegularExpression(@"^.*(?=.*[A-Z])(?=.*[0-9])(?=.*[!@#$%^&*\(\)_\-+=]).*$", ErrorMessage = "User_Password_must include at least 1 number, capital letter, small letter")]
        [StringLength(20, MinimumLength = 9, ErrorMessage = "length err")]
        [DataType(DataType.Password)]
        public string password { get; set; }
        public bool IsSuspended { get; set; }

        //navigational properties

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public User()
        {
            Comments = new List<Comment>();
            Roles = new List<Role>();
            Blogs = new List<Blog>();
        }*/

    }








