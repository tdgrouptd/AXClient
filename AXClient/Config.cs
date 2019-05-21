using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Security.Principal;

namespace AXClient
{
    class Config
    {
        public string local_ip = "";
        public string pred_ip = "";
        public int pred_port = -1;
        public int dialer = -1;
        public bool remote_agent = false;
        public int line = -1;
        public string ext = "";
        public string view_url = "";
        public string pc_name = "";
        public string pc_user = "";
        public string domain = "";
        public string pwd = "";
        public bool use_internal_sip = false;
        public string sip_server = "";
        public int sip_port = -1;
        public string sip_login = "";
        public string sip_password = "";
        public string axserver = "";
        private User user;

        public Config(User u)
        {
            user = u;
            local_ip = GetLocalIPAddress();
            string[] temp = Convert.ToString(WindowsIdentity.GetCurrent().Name).Split('\\');
            domain =  temp[0];
            pc_user = temp[1];
            pc_name = System.Environment.MachineName;
            pwd = Environment.CurrentDirectory;
            load_conf_table();
        }

        private  void load_conf_table()
        {
            var db =new  DB(user);
            var dt = db.exec(string.Format(@"select axserver,dialer,line,extension,internal_sip_client,sip_server,sip_port,sip_login,sip_password,view_server
            from TelAdmin.dbo.axclient_conf 
            where pc_name ='{0}' and pc_domain ='{1}' and pc_user = '{2}'
            ",pc_name,domain,pc_user));
            if (dt.Rows.Count==0)
            {
                throw new Exception("can't load client config");
            }
            else
            {
                try
                {
                    axserver = dt.Rows[0]["axserver"].ToString();
                    line = Int32.Parse(dt.Rows[0]["line"].ToString());
                    dialer = Int32.Parse(dt.Rows[0]["dialer"].ToString());
                    ext = dt.Rows[0]["extension"].ToString();
                    use_internal_sip = Boolean.Parse(dt.Rows[0]["internal_sip_client"].ToString());
                    sip_server = dt.Rows[0]["sip_server"].ToString();
                    sip_port = Int32.Parse(dt.Rows[0]["sip_port"].ToString());
                    sip_login = dt.Rows[0]["sip_login"].ToString();
                    sip_password = dt.Rows[0]["sip_password"].ToString();
                    view_url = dt.Rows[0]["view_server"].ToString();
                    dt = db.exec(string.Format(@"select dialeraddress,portid 
                    from teladmin.dbo.PredictiveDialers
                    where dialer={0}", dialer));
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("can't load predictive config");
                    }
                    else
                    {
                        pred_port = Int32.Parse(dt.Rows[0]["portid"].ToString());
                        pred_ip = dt.Rows[0]["dialeraddress"].ToString();

                    }
                }
                catch(Exception  ee)
                {
                    throw new Exception("wrong client config");
                }
                if (use_internal_sip && (sip_server.Trim() == "" || sip_login.Trim() == "" || sip_password.Trim() == "") || view_url.Trim()=="")
                {
                    throw new Exception("wrong sip config");
                }
            }
            
        }
        

        private  string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

    }
}
