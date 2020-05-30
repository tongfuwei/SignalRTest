using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignalRTest_Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string ServerUrl = "http://localhost:8888";

        public IDisposable SignalR { get; set; }

        private void button1_Click(object sender, EventArgs e)
        {
            WriteToConsole("正在启动服务...");
            Task.Run(() => StartServer()); // 异步启动SignalR服务
        }

        private void StartServer()
        {
            try
            {
                SignalR = WebApp.Start(ServerUrl);  // 启动SignalR服务
            }
            catch (Exception ex)
            {
                WriteToConsole("服务开启失败,原因:" + ex.Message);
                return;
            }
            WriteToConsole("服务已经成功启动，地址为：" + ServerUrl);
            return;
        }

        private delegate void WriteToConsoleDe(string msg);
        /// <summary>
        /// 将消息添加到消息列表中
        /// </summary>
        /// <param name="message"></param>
        public void WriteToConsole(string message)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new Action<string>((string msg) => richTextBox1.AppendText(message + Environment.NewLine)), message);
                return;
            }

            richTextBox1.AppendText(message + Environment.NewLine);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _ = StopServer();
        }

        private async Task StopServer()
        {
            if (SignalR != null)
            {
                //向客户端广播消息
                IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<SignalRHub>();
                await hubContext.Clients.All.AddMessage("服务端已关闭");

                //释放对象
                SignalR.Dispose();
                SignalR = null;

                WriteToConsole("服务端已关闭");
            }
        }

        /// <summary>
        /// 获取本地IP地址信息
        /// </summary>
        private string GetAddressIP()
        {
            string ip = null;
            string hostName = Dns.GetHostName();
            IPAddress[] iPAddresses = Dns.GetHostAddresses(hostName);
            foreach (IPAddress ipa in iPAddresses)
            {
                if (ipa.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    ip = ipa.ToString();
            }
            return ip;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ServerUrl = "http://" + GetAddressIP() + ":8888";
        }
    }
}
