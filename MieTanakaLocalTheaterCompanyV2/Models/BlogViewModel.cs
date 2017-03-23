using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MieTanakaLocalTheaterCompanyV2.Models
{

        public class BlogViewModel {
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



        public IEnumerable<Comment> Comments { get; set; }
        public IEnumerable<Blog> Blogs { get; set; }


    }
    
}