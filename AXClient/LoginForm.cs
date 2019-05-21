using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AXClient
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        public User user;

        private void button1_Click(object sender, EventArgs e)
        {
            login();
           
        }
        private void login()
        {
            try
            {
                user = new User(textBox1.Text.ToUpper(), textBox2.Text);
                this.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                login();
            }
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Top = 100;
            this.Left = 50;
        }
    }
}
