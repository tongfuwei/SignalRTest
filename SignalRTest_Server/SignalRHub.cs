using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRTest_Server
{
    public class SignalRHub : Hub
    {
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        /// <summary>
        /// 信息广播
        /// </summary>
        /// <param name="identify">唯一标识</param>
        /// <param name="message">信息</param>
        [HubMethodName("Send")]
        public void Send(string message)
        {
            Clients.All.AddMessage(Context.ConnectionId+":" + message);
        }

        [HubMethodName("TargetSend")]
        public void Send(string who, string message)
        {
            string name = Context.QueryString["UserName"];

            foreach (var connectionId in _connections.GetConnections(who))
            {
                Clients.Client(connectionId).AddMessage(name + ": " + message);
            }
        }

        public override Task OnConnected()
        {
            Program.serverFrm.WriteToConsole("客户端连接ID:" + Context.ConnectionId + "用户名为:" + Context.QueryString["UserName"]);
            string name = Context.QueryString["UserName"];
            _connections.Add(name, Context.ConnectionId);
            Clients.All.AddMessage(name + "登录了");
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Program.serverFrm.WriteToConsole("客户端退出ID:" + Context.ConnectionId + "用户名为:" + Context.QueryString["UserName"]);
            string name = Context.QueryString["UserName"];
            _connections.Remove(name, Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            Program.serverFrm.WriteToConsole("客户端退出ID:" + Context.ConnectionId + "用户名为:" + Context.QueryString["UserName"]);
            string name = Context.QueryString["UserName"];
            _connections.Remove(name, Context.ConnectionId);
            return base.OnReconnected();
        }
    }
}
