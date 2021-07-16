namespace BigSchool.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Course")]
    public partial class Course
    {
        public Course()
        {
            Attendance = new HashSet<Attendance>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string LectureId { get; set; }

        [Required]
        [StringLength(250)]
        public string Place { get; set; }

        public DateTime DateTime { get; set; }

        public int CategoryId { get; set; }

        public virtual ICollection<Attendance> Attendance { get; set; }

        public virtual Category Category { get; set; }
        public string Name;
        public List<Category> ListCategory = new List<Category>();
        public string LectureName;
        public bool isLogin = false;
        public bool isShowGoing = false;
        public bool isShowFollow = false;

    }
}
