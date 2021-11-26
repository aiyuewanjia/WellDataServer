using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WellDataService.Dtos.RequestDto;
using Newtonsoft.Json;

namespace WellDataService.Services
{
    public class ReciveActions
    {
        public EndPoint point;
        public int length;
        public byte[] buffer;
        private string url = "http://localhost:5000/Well/AddDeviceData";
        public void ReciveAction()
        {
            string message = Encoding.UTF8.GetString(buffer, 0, length);
            //086907803900004311 22 33 44 55 66 77 88 99
            string Imei = message.Substring(1, 15);
            string[] str = message.Substring(16, message.Length - 16).Split(" ");
            AddDeviceDataRequestDto data = new AddDeviceDataRequestDto() { Imei = Imei, Temperature = str[0], OffsetValue = float.Parse(str[1]), IsAlarm = false };
            GetStringByHttpPostRaw(data);
        }
        public void GetStringByHttpPostRaw(AddDeviceDataRequestDto data)
        {
            try
            {
                Type t = typeof(AddDeviceDataRequestDto);
                var propeties = t.GetProperties();
                HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
                reqest.Method = "POST";
                reqest.ContentType = "application/json";
                Stream stream = reqest.GetRequestStream();
                StringBuilder sb = new StringBuilder();
                var datajson = JsonConvert.SerializeObject(data);
                byte[] bs = System.Text.Encoding.UTF8.GetBytes(datajson);
                stream.Write(bs, 0, bs.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)reqest.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                sr.ReadToEnd();
                response.Close();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
