using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Course
    {
        public Course()
        {
            Classes = new HashSet<Class>();
        }

        public uint CourseId { get; set; }
        public string DeptSubject { get; set; } = null!;
        public uint CourseNum { get; set; }
        public string Name { get; set; } = null!;

        public virtual Department DeptSubjectNavigation { get; set; } = null!;
        public virtual ICollection<Class> Classes { get; set; }
    }
}
