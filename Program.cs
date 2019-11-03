using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace AutoEcnuNet
{
    class Program
    {
        static void Main(string[] args)
        {
            switch(args[0])
            {
                case "logout":
                    NetHelper.Logout();
                    break;
                case "login":
                    NetHelper.Login(args[1], args[2]);
                    break;
            }
        }
    }
    class NetHelper
    {
        static HttpClient client = new HttpClient();
        static string url = "https://login.ecnu.edu.cn/srun_portal_pc.php";
        static string loginUrl = "https://login.ecnu.edu.cn/include/auth_action.php";
        struct HtmlState
        {
            public string ip;
            public string username;
            public string password;
        }
       
        static HtmlState GetLoginState()
        {
            HtmlState state=new HtmlState();
            var res=client.GetAsync(url).Result;
            var html = res.Content.ReadAsStringAsync().Result;
            state.ip=Regex.Match(html,"(?<=<input type=\"hidden\" name=\"user_ip\" id=\"user_ip\" value=\").*(?=\">)").ToString();
            state.username = Regex.Match(html,"(?<=<input type=\"hidden\" name=\"username\" id=\"username\" value=\").*(?=\">)").ToString();
            return state;
        }
        public static bool Logout()
        {
            var state=GetLoginState();
            var values = new Dictionary<string, string>
            {
                { "action", "auto_logout" },
                { "user_ip", state.ip },
                {"username",state.username }
            };
            var content = new FormUrlEncodedContent(values);
            var response = client.PostAsync(url, content).Result;
            return true;
        }
        public static bool Login(string username,string password)
        {
            var values = new Dictionary<string, string>
            {
                { "action", "login" },
                { "username",username },
                {"password",password },
                {"ac_id","1" }
            };
            var content = new FormUrlEncodedContent(values);
            var response = client.PostAsync(loginUrl, content).Result;
            return true;
        }
    }
}
