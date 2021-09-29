namespace BotApi.Models
{
    /// <summary>
    /// the common response struct.
    /// </summary>
    public class ApiRes
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRes"/> class.
        /// </summary>
        /// <param name="ok"> the return ok.</param>
        /// <param name="msg"> the return msg.</param>
        /// <param name="data"> the return data.</param>
        public ApiRes(bool ok, string msg, object data)
        {
            this.Ok = ok;
            this.Msg = msg;
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets a value indicating whether handled as expect.
        /// </summary>
        public bool Ok { get; set; }

        /// <summary>
        /// Gets or sets the return msg.
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// Gets or sets the return data.
        /// </summary>
        public object Data { get; set; }
    }
}
