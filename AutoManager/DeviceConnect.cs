using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AutoManager
{
    public class DeviceConnect
    {
        public string seri = "";
     
        public string getSeri(string ipaddress)
        {
            return HTTPGet("http://" + ipaddress + ":1710/app/serial");
        }
        public string SetProxy(string ipaddress,string ipPC , int port,bool onOFF)
        {
            int ienable = 0;
            if (onOFF)
            {
                ienable = 1;
            }
            return HTTPGet("http://" + ipaddress + ":1710/app/socks5?ipaddr="+ipPC + "port="+ port+ "setEnable="+ienable);
        }


        public string HTTPGet(string url)
        {
            string result;
            try
            {
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.Method = "GET";
                HttpWebResponse httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
                Stream responseStream = httpWebResponse.GetResponseStream();
                StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
                string text = streamReader.ReadToEnd();
                result = text.Replace("<br>", "\n");
            }
            catch
            {
                result = "";
            }
            return result;
        }
    }
}
