using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Socket红外通信
{
    public partial class Form1 : Form
    {
        private string SendProtocol= "ee e1 01 55 ff fc fd ff";
        private byte[] SendByte = new byte[8];
        private byte[] InputDate = new byte[4000];
        private Socket WatherSocket;
        private Socket SendSocket;
       
        public Form1()
        {
            InitializeComponent();
            
        }

        private void Close_Btn_Click(object sender, EventArgs e)
        {
            serialPort1.Close();
            SendTimer.Stop();
        }

        private void Open_Btn_Click(object sender, EventArgs e)
        {
            serialPort1.PortName = comboBox1.Text.Trim();
            serialPort1.BaudRate =Convert.ToInt32(textBox1.Text.Trim());
           
            serialPort1.Open();
            SendTimer.Start();
        
           
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
            IPEndPoint iPEnd = new IPEndPoint(IPAddress.Parse("192.168.5.101"), 40000);
            WatherSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            try
            {
                WatherSocket.Bind(iPEnd);
                WatherSocket.Listen(12);
            }
            catch
            {
                MessageBox.Show("连接发生错误");
            }
            Thread th_wait = new Thread(WaitClient);
            th_wait.IsBackground = true;
            th_wait.Start();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
              
        Thread.Sleep(300);
            serialPort1.Read(InputDate, 0, (4000>serialPort1.BytesToRead? serialPort1.BytesToRead:4000));
            SendMessage();
            label1.Text= (InputDate[11] * 256 + InputDate[12]).ToString();
           
        }

        private void SendTimer_Tick(object sender, EventArgs e)
        {
            SendTimer.Stop();
           
            try
            {
            
                string[] SendMessageContent = SendProtocol.Split(' ');
                for (int i = 0; i < 8; i++)
                {
                    SendByte[i] = Convert.ToByte(SendMessageContent[i], 16);
                }
                serialPort1.Write(SendByte, 0, SendByte.Length);
            }
            catch(Exception ex)
            {
                MessageBox.Show("发送错误");
            }
            SendTimer.Start();



        }
        private void SendMessage()//发送消息
        {
         
            try
            {
       
                SendSocket.Send(InputDate);
            }
            catch
            {
                MessageBox.Show("发送失败");
            }
        }
        private void WaitClient()
        {
            while(true)
            {
                SendSocket = WatherSocket.Accept();
            }
       

        }
      
    }
}
