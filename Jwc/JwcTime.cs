namespace Jwc
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Use for get Jwc time.
    /// </summary>
    public static class JwcTime
    {
        /// <summary>
        /// Gets 开学第一周周一.
        /// </summary>
        public static DateTime TermStart { get; private set; } = Config.TermStart;

        /// <summary>
        /// Gets 本周是学期第几周.
        /// </summary>
        public static int Week => ((DateTime.Now - TermStart).Days / 7) + 1;

        /// <summary>
        /// 获取这一周的周一.
        /// </summary>
        /// <param name="jwcWeek">week start from 1.</param>
        /// <returns>date.</returns>
        public static DateTime GetMondayOfWeek(int jwcWeek) => TermStart + new TimeSpan(7 * (jwcWeek - 1), 0, 0, 0);

        /// <summary>
        /// 获取某节课的上课时间.
        /// </summary>
        /// <param name="week">那节课是第几周.</param>
        /// <param name="day">那节课是周几.</param>
        /// <param name="section">第几节课(1-13).</param>
        /// <returns>那节课的开始时间.</returns>
        public static DateTime GetCourseStartDate(int week, DayOfWeek day, int section) => GetDate(week, day) + GetCourseStartTime(section);

        /// <summary>
        /// 获取某节课的上课时间.
        /// </summary>
        /// <param name="day">开学第几天.</param>
        /// <param name="section">第几节课(1-13).</param>
        /// <returns>那节课的上课时间.</returns>
        public static DateTime GetCourseStartDate(int day, int section) => GetDate(day) + GetCourseStartTime(section);

        /// <summary>
        /// 获取某节课的下课时间.
        /// </summary>
        /// <param name="week">那节课是第几周.</param>
        /// <param name="day">那节课是周几.</param>
        /// <param name="section">第几节课(1-13).</param>
        /// <returns>那节课的下课时间.</returns>
        public static DateTime GetCourseEndDate(int week, DayOfWeek day, int section) => GetDate(week, day) + GetCourseEndTime(section);

        /// <summary>
        /// 获取某节课的下课时间.
        /// </summary>
        /// <param name="day">那节课是开学第几天.</param>
        /// <param name="section">第几节课(1-13).</param>
        /// <returns>那节课的下课时间.</returns>
        public static DateTime GetCourseEndDate(int day, int section) => GetDate(day) + GetCourseEndTime(section);

        /// <summary>
        /// 获取上课时间.
        /// </summary>
        private static TimeSpan GetCourseStartTime(int section)
        {
            var dic = new Dictionary<int, string>
            {
                { 1, "08:00:00" },
                { 2, "08:50:00" },
                { 3, "09:55:00" },
                { 4, "10:45:00" },
                { 5, "11:35:00" },
                { 6, "14:00:00" },
                { 7, "14:50:00" },
                { 8, "15:55:00" },
                { 9, "16:45:00" },
                { 10, "17:35:00" },
                { 11, "19:00:00" },
                { 12, "19:50:00" },
                { 13, "20:40:00" },
            };
            return TimeSpan.Parse(dic[section]);
        }

        private static TimeSpan GetCourseEndTime(int section)
        {
            var map = new Dictionary<int, string>
            {
                { 1, "08:45:00" },
                { 2, "09:35:00" },
                { 3, "10:40:00" },
                { 4, "11:30:00" },
                { 5, "12:20:00" },
                { 6, "14:45:00" },
                { 7, "15:35:00" },
                { 8, "16:25:00" },
                { 9, "17:30:00" },
                { 10, "18:20:00" },
                { 11, "19:45:00" },
                { 12, "20:35:00" },
                { 13, "21:25:00" },
            };
            return TimeSpan.Parse(map[section]);
        }

        /// <summary>
        /// 获取日期.
        /// </summary>
        /// <param name="week">开学第几周.</param>
        /// <param name="dow">一周中的第几天.</param>
        /// <returns>对应日期.</returns>
        private static DateTime GetDate(int week, DayOfWeek dow) => TermStart + new TimeSpan((7 * (week - 1)) + Dow2Order(dow) - 1, 0, 0, 0);

        /// <summary>
        /// 获取日期.
        /// </summary>
        /// <param name="day">The day after this term, start from 1.</param>
        /// <returns>the datetime of the that day.</returns>
        private static DateTime GetDate(int day) 
            => TermStart + new TimeSpan(day - 1, 0, 0, 0);

        /// <summary>
        /// 将周几按它的数值返回
        /// </summary>
        /// <param name="dow"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static int Dow2Order(DayOfWeek dow)
            => dow switch
            {
                DayOfWeek.Monday => 1,
                DayOfWeek.Tuesday => 2,
                DayOfWeek.Wednesday => 3,
                DayOfWeek.Thursday => 4,
                DayOfWeek.Friday => 5,
                DayOfWeek.Saturday => 6,
                DayOfWeek.Sunday => 7,
                _ => throw new NotImplementedException(),
            };
    }
}
