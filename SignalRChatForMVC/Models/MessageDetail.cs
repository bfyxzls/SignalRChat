using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChatForMVC.Model
{
    public class MessageDetail
    {
        public string UserID { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

    }
}