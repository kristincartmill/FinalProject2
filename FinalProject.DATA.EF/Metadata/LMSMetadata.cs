using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.DATA.EF//.Metadata
{
    #region Course Metadata

    
    public class CourseMetadata
    {

        [Display(Name = "Course")]
        public string CourseName { get; set; }

        [Display(Name = "Course Description")]
        public string CourseDescription { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
    [MetadataType(typeof(CourseMetadata))]
    public partial class Course
    {

    }
    #endregion

    #region Course Completion Metadata
    public class CourseCompletionMetadata
    {
        public string UserId { get; set; }
        public int CourseId { get; set; }

        [Display(Name = "Date Completed")]
        public System.DateTime DateCompleted { get; set; }
    }
    [MetadataType(typeof(CourseCompletionMetadata))]
    public partial class CourseCompletion
    {

    }
    #endregion

    #region Lesson Metadata
    public class LessonMetadata
    {

        [Display(Name = "Lesson Title")]
        public string LessonTitle { get; set; }

        [Display(Name = "Course ID")]
        public int CourseId { get; set; }

        [Display(Name = "Lesson Description")]
        [UIHint("MultilineText")]
        public string Introduction { get; set; }

        [Display(Name = "Video")]
        public string VideoURL { get; set; }

        [Display(Name = "PDF Document")]
        public string PdfFilename { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

    }
    [MetadataType(typeof(LessonMetadata))]
    public partial class Lesson
    {

    }
    #endregion

    #region Lesson View Metadata
    public class LessonViewMetadata
    {      
        public string UserId { get; set; }
        public string LessonId { get; set; }

        [Display(Name = "Date Completed")]
        public System.DateTime DateViewed { get; set; }
    }
    [MetadataType(typeof(LessonViewMetadata))]
    public partial class LessonView
    {

    }
    #endregion
}
