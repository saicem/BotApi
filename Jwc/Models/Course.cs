namespace Jwc.Models
{
    using System;

    /// <summary>
    /// the course.
    /// </summary>
    public class Course
    {
        /// <summary>
        /// 课程名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public string Room { get; set; }

        /// <summary>
        /// 开始周
        /// </summary>
        public int WeekStart { get; set; }

        /// <summary>
        /// 结束周
        /// </summary>
        public int WeekEnd { get; set; }

        /// <summary>
        /// 有几周
        /// </summary>
        public int WeekSpan
        {
            get { return this.WeekEnd - this.WeekStart + 1; }
        }

        /// <summary>
        /// 开始节
        /// </summary>
        public int SectionStart { get; set; }

        /// <summary>
        /// 结束节
        /// </summary>
        public int SectionEnd { get; set; }

        /// <summary>
        /// 几节课
        /// </summary>
        public int SectionSpan
        {
            get { return this.SectionEnd - this.SectionStart + 1; }
        }

        /// <summary>
        /// 星期几
        /// </summary>
        public DayOfWeek DayOfWeek { get; set; }

        /// <summary>
        /// 教师
        /// </summary>
        public string Teacher { get; set; }

        /// <summary>
        /// 学分
        /// </summary>
        public string Credit { get; set; }

        /// <summary>
        /// 是否评教
        /// </summary>
        public string Status { get; set; }
    }
}
