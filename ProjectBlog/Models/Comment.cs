using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectBlog.Models
{
    public class Comment
    {
        public int CommentID { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        [Required]
        public DateTime CommentDate { set; get; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int PostID { get; set; }
        [Required]
        public bool IsDeleted { set; get; }

        public virtual User User { set; get; }
        public virtual Post Post { set; get; }
    }
}