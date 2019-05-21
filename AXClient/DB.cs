using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data;
using Newtonsoft.Json;
using AXClient;

namespace AXClient
{

    class DBResult
    {
        public bool result;
        public List<string> columns;
        public List<Dictionary<string, object>> data;
        public int rows;
        public string err;
    }
    class DB
    {
        User user;
        public DB(User u)
        {
            user = u;
        }
        public DataTable exec(string query)
        {
            query = query.Replace("\"", "\\\"").Replace("\r\n"," ");
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            DataTable dt = new DataTable();
            WebClient wc = new WebClient();
            wc.Headers.Add("content-type", "application/json; charset=utf8");
            string data = "{\"user\":\""+user.username+"\",\"password\":\""+user.password+"\",\"method\":\"sql\",\"query\":\""+query+"\"}";

            string answer = wc.UploadString("https://tdirect.msk.ru/axview/api/","POST",data);
            DBResult res = JsonConvert.DeserializeObject<DBResult>(answer);
            if (res.result)
            {
                foreach (var column in res.columns)
                {
                    dt.Columns.Add(column);
                }
                foreach (Dictionary<string, object> dct in res.data)
                {
                    dt.Rows.Add();
                    foreach (string key in dct.Keys)
                    {
                        dt.Rows[dt.Rows.Count - 1][key] = dct[key];
                    }
                }
            }
            else
            {
                throw new Exception("ошибка запроса");
            }
            return dt;
        }

    }
}
