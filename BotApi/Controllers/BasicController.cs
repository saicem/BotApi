namespace BotApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Serilog;
    using BotApi.Models;
    using CalCreate;
    using Jwc;
    using System.Linq;

    /// <summary>
    /// the basic controller.
    /// </summary>
    [ApiController]
    [Route("api/jwc")]
    public class BasicController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IMemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicController"/> class.
        /// </summary>
        /// <param name="configuration"> config svc.</param>
        public BasicController(IConfiguration configuration,IMemoryCache cache)
        {
            this.configuration = configuration;
            this.cache = cache;
        }

        /// <summary>
        /// test if the api can be use.
        /// </summary>
        /// <returns>pong.</returns>
        [HttpGet("ping")]
        public ApiRes Ping() => new(true, "pong", null);

        /// <summary>
        /// get calendar infos.
        /// </summary>
        /// <param name="username"> jwc username.</param>
        /// <param name="password"> jwc password.</param>
        /// <returns>A <see cref="Task{TResult}"/> get calendar infos.</returns>
        [HttpPost("cal")]
        public async Task<ApiRes> GetCalAsync([Required] string username, [Required] string password)
        {
            Log.Information($"course/cal > {username} {password}");
            var user = new JwcUser(username, password);
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
                    SUMMARY = course.Name,
                    DESCRIPTION = $"老师:{course.Teacher}\\n学分:{course.Credit}",
                    LOCATION = course.Room,
                    RRULE = $"FREQ=WEEKLY;INTERVAL=1;BYDAY={DowStr.Get(course.DayOfWeek)};COUNT={course.WeekSpan}",
                    UID = $"{DateTime.Now:s}-{i}",
                });
            }

            // TODO 缓存策略 随机文件名 定期删除
            var fileName = $"{username}.ics";
            var path = $"CalendarFiles/{fileName}";
            CalCreater.CalendarMake(eventLs, path, username);
            var localhost = this.configuration["localhost"];
            var token = GetRandomString(32, true, true, true, false, string.Empty);
            var expireTime = DateTime.Now.AddSeconds(600);
            cache.Set(token, fileName, expireTime);
            return new ApiRes(true, "Success", $"{localhost}/api/jwc/cal/{token}");
        }

        [HttpGet("cal/{token}")]
        public IActionResult GetCal(string token)
        {
            var success = cache.TryGetValue(token, out var fileName);
            if (!success)
            {
                return StatusCode(406);
            }
            var stream = System.IO.File.OpenRead($"./CalendarFiles/{fileName}");
            return File(stream, "application/octet-stream", $"course.ics");
        }

        /// <summary>
        /// verify the user is valid. if valid, get his name.
        /// </summary>
        /// <param name="username"> jwc username.</param>
        /// <param name="password"> jwc password.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("verify")]
        public async Task<ApiRes> JwcVerifyAsync([Required] string username, [Required] string password)
        {
            var user = new JwcUser(username, password);
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
        public async Task<ApiRes> JwcCourseJsonAsync([Required] string username, [Required] string password, int weekOrder = 0)
        {
            Log.Information($"course/json > {username} {password}");
            var user = new JwcUser(username, password);
            var courseLs = await user.GetCoursesAsync();
            if (courseLs == null)
            {
                return new ApiRes(false, "错误的账密", null);
            }
            if (weekOrder == 0)
            {
                return new ApiRes(true, "获取成功", courseLs);
            }
            var retCourses = from course in courseLs where course.WeekStart <= weekOrder && course.WeekEnd >= weekOrder select course;
            return new ApiRes(true, "获取成功", retCourses);
        }

        private static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
        {
            byte[] b = new byte[4];
            new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
            Random rand = new(BitConverter.ToInt32(b, 0));
            string s = null, str = custom;
            if (useNum == true) { str += "0123456789"; }
            if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
            if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
            if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
            for (int i = 0; i < length; i++)
            {
                s += str.Substring(rand.Next(0, str.Length - 1), 1);
            }
            return s;
        }
    }
}
