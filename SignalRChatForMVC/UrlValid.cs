using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SignalR.Core.Model;
using SignalRChatForMVC.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SignalRChatForMVC
{

    [HubName("UrlHubZzl")]
    public class UrlValid : Hub
    {
        static List<CurrentUser> ConnectedUsers = new List<CurrentUser>();

        public void Connect(string userID)
        {
            var id = Context.ConnectionId;
            if (ConnectedUsers.Count(x => x.UserID == userID) == 0)
            {
                ConnectedUsers.Add(new CurrentUser
                {
                    ConnectionId = id,
                    UserID = userID,
                });
                // Clients.Caller.onConnected(id, userID, url);
                // Clients.AllExcept(id).onNewUserConnected(id, userID);//向所有客户端发请求,除了当前用户之外
                Clients.All.onNewUserConnected(userID);
                Clients.All.onUserList(ConnectedUsers);
                Clients.All.onConnected(id, userID, null);
                // Clients.Client(id).onNewUserConnected(id, userID, ConnectedUsers);//向指定客户端发请求
            }
            else
            {

                //  Clients.Caller.onConnected(id, userID, url);
                Clients.Client(id).onExistUserConnected(id, userID);
                // Clients.AllExcept(id).onExistUserConnected(id, userID);
            }
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void Exit(string userID)
        {
            var id = Context.ConnectionId;

            OnDisconnected();
            //    Clients.Caller.onConnected(id, userID, "");
            Clients.All.onUserList(ConnectedUsers);
            Clients.Client(id).onExit(userID);
        }

        public void Message(string fromUser, string toUser, string message)
        {
            var id = Context.ConnectionId;
            var toConnection = ConnectedUsers.FirstOrDefault(x => x.UserID == toUser);
            if (toConnection == null)
            {
                Clients.Client(id).onMessageError("你的接收方不在线...");
            }
            else
            {
                toConnection.MessageCount += 1;
                toConnection.MessageList.Add(new Message
                {
                    ID = Guid.NewGuid().ToString(),
                    Info = message,
                    ToUser = toUser,
                    FromUser = fromUser,
                    IsNew = 1,
                });
                Clients.Client(toConnection.ConnectionId).onMessage(toConnection.MessageCount);
                Clients.Client(toConnection.ConnectionId).onMessageList(toConnection.MessageList);
            }
        }
        /// <summary>
        /// 断开
        /// </summary>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnDisconnected()
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);

                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.UserID);

            }
            return base.OnDisconnected();
        }

    }
}
