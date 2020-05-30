using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignalRTest_Client
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
        }

        private string ServerUrl;
        private string UserName;

        private static IHubProxy HubProxy { get; set; }
        public static HubConnection Connection { get; set; }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Connection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Disconnected)
                return;
            if (!string.IsNullOrWhiteSpace(tbTarget.Text))
                HubProxy.Invoke("TargetSend",tbTarget.Text, tbMessage.Text);
            else
                HubProxy.Invoke("Send", tbMessage.Text);
        }

        private void button1_Click(object sender,EventArgs e)
        {
            ServerUrl = "http://" + tbIP.Text.Trim().ToString();
            UserName = tbMyName.Text.Trim().ToString();
            //ServerUrl = "http://localhost:8888";
            WriteToConsole("开始连接服务器" + ServerUrl);
            InitHub();
        }

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

        private void Form1_Load(object sender, EventArgs e)
        {
            //InitHub();
        }
        
        private async void InitHub()
        {
            var queryStringData = new Dictionary<string, string>();
            queryStringData.Add("UserName", UserName);
            //创建连接对象
            Connection = new HubConnection(ServerUrl,queryStringData);
            //绑定一个集线器
            HubProxy = Connection.CreateHubProxy("SignalRHub");
            HubProxy.On("AddMessage", (m) =>
            {
                WriteToConsole(m);
            }
                );
            try
            {
                //开始连接
                await Connection.Start();
            }
            catch (Exception ex)
            {
                WriteToConsole( "服务器未连接上,异常为:" + ex.Message);
                return;
            }
            WriteToConsole("服务器已连接上");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Connection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
            {
                Connection.Stop(new TimeSpan(5));
                WriteToConsole("服务器已断开");
            }
        }
    }
}
