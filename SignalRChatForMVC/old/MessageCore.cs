using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SignalRChatForMVC.Model;
namespace SignalRChatForMVC
{
    /// <summary>
    /// SignalR服务端核心功能类,MessageHub为路由名称，在客户端需要与它匹配
    /// 服务端方法将被客户端调用...
    /// </summary>
    [HubName("MessageHub")]
    public class MessageCore : Hub
    {
        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();
        public void Connect(string userid)
        {
            var id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {
                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserID = userid });

                Clients.Caller.onConnected(id, userid, ConnectedUsers, CurrentMessage);

                Clients.AllExcept(id).onNewUserConnected(id, userid);
            }
        }
        /// <summary>
        /// 全站的消息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        public void SendMessageToAll(string userid, string message)
        {
            AddMessageinCache(userid, message);
            ConnectedUsers.ForEach(i => i.MessageCount += 1);
            Clients.All.messageReceived(userid, message);
            //在线的客户端统计信息更新,调用所有客户端的消息统计方法，这由服务器端发起
            Clients.All.printMessageTotal(ConnectedUsers.FirstOrDefault(i => i.UserID == userid).MessageCount);
        }
        /// <summary>
        /// 私有的消息
        /// </summary>
        /// <param name="toUserId"></param>
        /// <param name="message"></param>
        public void SendPrivateMessage(string toUserId, string message)
        {
            string fromUserId = Context.ConnectionId;

            var toUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == toUserId);
            var fromUser = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null && fromUser != null)
            {
                // send to
                Clients.Client(toUserId).sendPrivateMessage(fromUserId, fromUser.UserName, message);

                // send to caller user
                Clients.Caller.sendPrivateMessage(toUserId, fromUser.UserName, message);
            }
        }
        /// <summary>
        /// 断开链接
        /// </summary>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnDisconnected()
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserName);
             
            }
            return base.OnDisconnected();
        }
        /// <summary>
        /// 消息加入缓存
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        private void AddMessageinCache(string userid, string message)
        {
            CurrentMessage.Add(new MessageDetail { UserID = userid, Message = message });

            if (CurrentMessage.Count > 100)
            {
                CurrentMessage.RemoveAt(0);
            }
        }
    }
}
