using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Submission
    {
        public string StudentuId { get; set; } = null!;
        public uint AssignmentId { get; set; }
        public DateTime SubmissionTime { get; set; }
        public string Contents { get; set; } = null!;
        public uint Score { get; set; }

        public virtual Assignment Assignment { get; set; } = null!;
        public virtual Student Studentu { get; set; } = null!;
    }
}
