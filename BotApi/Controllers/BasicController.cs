namespace BotApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using BotApi.Models;
    using CalCreate;
    using Jwc;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Serilog;

    /// <summary>
    /// the basic controller.
    /// </summary>
    [ApiController]
    [Route("api/jwc")]
    public class BasicController : ControllerBase
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicController"/> class.
        /// </summary>
        /// <param name="configuration"> config svc.</param>
        public BasicController(IConfiguration configuration) => this.configuration = configuration;

        /// <summary>
        /// test if the api can be use.
        /// </summary>
        /// <returns>pong.</returns>
        [HttpGet("ping")]
        public ApiRes Ping() => new(true, "pong", null);

        /// <summary>
        /// get calendar infos.
        /// </summary>
        /// <param name="userName"> jwc username.</param>
        /// <param name="password"> jwc password.</param>
        /// <returns>A <see cref="Task{TResult}"/> get calendar infos.</returns>
        [HttpPost("cal")]
        public async Task<ApiRes> GetCalAsync([Required] string userName, [Required] string password)
        {
            Log.Information($"course/cal > {userName} {password}");
            var user = new JwcUser(userName, password);
            var courseLs = await user.GetCoursesAsync();
            if (courseLs == null)
            {
                return new ApiRes(false, "wrong username or passwd", null);
            }

            List<Vevent> eventLs = new();
            for (int i = 0; i < courseLs.Count; i++)
            {
                Jwc.Models.Course course = courseLs[i];
                eventLs.Add(new Vevent
                {
                    DTSTAMP = DateTime.Now,
                    DTSTART = JwcTime.GetCourseStartDate(course.WeekStart, course.DayOfWeek, course.SectionStart),
                    DTEND = JwcTime.GetCourseEndDate(course.WeekStart, course.DayOfWeek, course.SectionEnd),
                    SUMMARY = course.CourseName,
                    DESCRIPTION = $"老师:{course.Teacher}\\n学分:{course.Credit}",
                    LOCATION = course.Room,
                    RRULE = $"FREQ=WEEKLY;INTERVAL=1;BYDAY={DowStr.Get(course.DayOfWeek)};COUNT={course.WeekSpan}",
                    UID = $"{DateTime.Now:s}-{i}",
                });
            }

            // TODO 缓存策略 随机文件名 定期删除
            var fileName = $"{userName}.ics";
            var path = $"CalendarFiles/{fileName}";
            CalCreater.CalendarMake(eventLs, path, userName);
            var localhost = this.configuration["localhost"];
            return new ApiRes(true, "Success", $"{localhost}/cal/{fileName}");
        }

        /// <summary>
        /// verify the user is valid. if valid, get his name.
        /// </summary>
        /// <param name="userName"> jwc username.</param>
        /// <param name="password"> jwc password.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("verify")]
        public async Task<ApiRes> JwcVerifyAsync([Required] string userName, [Required] string password)
        {
            var user = new JwcUser(userName, password);
            var name = await user.GetValidAsync();
            if (name != null)
            {
                return new ApiRes(false, "有效用户", name);
            }

            return new ApiRes(true, "无效用户", null);
        }

        /// <summary>
        /// get the notice of jwc.
        /// </summary>
        /// <param name="username"> jwc username.</param>
        /// <param name="password"> jwc password.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("notice")]
        public async Task<ApiRes> JwcNoticeAsync([Required] string username, [Required] string password)
        {
            var user = new JwcUser(username, password);
            var notices = await user.GetNoticesAsync();
            return new ApiRes(true, null, notices);
        }

        /// <summary>
        /// get the serialized struct json of courses, for better use of other usage.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("json")]
        public async Task<ApiRes> JwcCourseJsonAsync([Required] string username, [Required] string password)
        {
            Log.Information($"course/json > {username} {password}");
            var user = new JwcUser(username, password);
            var courseLs = await user.GetCoursesAsync();
            if (courseLs == null)
            {
                return new ApiRes(false, "错误的账密", null);
            }
            return new ApiRes(true, "获取成功", courseLs);
        }
    }
}
