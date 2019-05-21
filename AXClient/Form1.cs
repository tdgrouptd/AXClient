using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AXClient;
using System.Net;
using System.Net.Sockets;

namespace AXClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        
        User user;
        Config conf;

        private void Form1_Load(object sender, EventArgs e)
        {
            login();

        }

        private void login()
        {
            try
            {
                do_login();
            }
            catch (Exception ee)
            {
                
                MessageBox.Show(ee.Message);
                do_login();
            }
        }
        private void do_login()
        {
            LoginForm login_form = new LoginForm();
            login_form.ShowDialog();
            user = login_form.user;
            conf = new Config(user);
            start_socket();
        }
        private void event_to_form(byte[] result)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                //code..
            }));
        }
        public string message="";

        private void start_socket()
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(socket_thread));
            t.Start();
        }
        private void socket_thread()
        {
            Socket sock = new Socket(SocketType.Stream, ProtocolType.Tcp);
            sock.ReceiveTimeout = 3000;
            sock.SendTimeout = 3000;
            IPAddress server;
            try
            {
                server = IPAddress.Parse(conf.axserver);
            }
            catch
            {
                server = Dns.GetHostAddresses(conf.axserver)[0];
            }
            IPEndPoint ipe = new IPEndPoint(server, 50901);
            sock.Connect(ipe);
            while (true)
            {
                try
                {
                    if (message == "")
                    {
                        sock.Send(Encoding.UTF8.GetBytes("k"));
                        byte[] type = new byte[1];
                        sock.Receive(type);
                        var evnt = Encoding.UTF8.GetString(type);
                        Console.WriteLine(evnt);
                    }
                    else
                    {
                        sock.Send(Encoding.UTF8.GetBytes("s"));//хочу послать сообщение
                        byte[] is_ready_buf = new byte[1];
                        sock.Receive(is_ready_buf);//принимаю готовность
                        var is_ready = Encoding.UTF8.GetString(is_ready_buf);
                        var message_bytes = Encoding.UTF8.GetBytes(message);
                        sock.Send(BitConverter.GetBytes(message_bytes.Length));//отправляю длину сообщения
                        var result = new byte[1];
                        sock.Receive(result);//принимаю результат
                        sock.Send(message_bytes);//отправляю сообщение
                        sock.Receive(result);//принимаю результат получения
                        var answ_len_bytes = new byte[4];
                        sock.Receive(answ_len_bytes);//получаю длину ответа на сообщение
                        var answ_len = BitConverter.ToInt32(answ_len_bytes, 0);
                        byte[] answer_bytes = new byte[answ_len];
                        sock.Send(Encoding.UTF8.GetBytes("y"));
                        while (sock.Available < answ_len)
                        {
                            System.Threading.Thread.Sleep(50);
                        }
                        sock.Receive(answer_bytes);//получаю ответ
                        var answer = Encoding.UTF8.GetString(answer_bytes);
                        message = "";
                        sock.Send(Encoding.UTF8.GetBytes("y"));


                    }
                }
                catch(Exception ee)
                {
                    Console.WriteLine(ee.Message);
                    sock.Disconnect(true);
                    Console.WriteLine("reconnecting");
                    sock.Connect(ipe);
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            message = textBox1.Text;
        }

    }
}
