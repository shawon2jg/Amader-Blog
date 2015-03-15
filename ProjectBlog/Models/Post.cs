using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace ProjectBlog.Models
{
    public class Post
    {
        public int PostID { get; set; }

        [Required]
        [StringLength(500)]
        public string Title { set; get; }

        [Required]
        [UIHint("~/Views/Shared/EditorTemplates/Html.cshtml"), AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        [Required]
        public DateTime PostedDate { set; get; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public bool IsPublished { set; get; }

        [Required]
        public int NoOfView { set; get; }

        public virtual User User { set; get; }
        public virtual ICollection<Comment> Comments { set; get; }
    }
}