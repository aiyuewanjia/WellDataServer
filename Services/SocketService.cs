using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WellDataService.Dtos.RequestDto;
using WellDataService.Models;

namespace WellDataService.Services
{
    public static class SocketService
    {
        static Socket server;
        static Dictionary<string,DeviceInteraction> _dictionary = new Dictionary<string, DeviceInteraction>();
        private static string url = "http://localhost:8010/Well/AddDeviceData";
        private static byte[] Oxbuffer = {0x01, 0x02, 0x00,0x00,0x00,0x08,0x79,0xCC};
        private static byte[] Fxbuffer = {0x01, 0x04, 0x00,0x10,0x00,0x03,0xB1,0xCE};
        
        public static void StartUp()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(new IPEndPoint(IPAddress.Any, 6000));//绑定端口号和IP
            Console.WriteLine("服务端已经开启");
            Thread t = new Thread(ReciveMethods);//开启接收消息线程
            t.Start();
        }
        
        private static void GetStringByHttpPostRaw(AddDeviceDataRequestDto data)
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
        
        private static void ReciveMethods()
        {
            while (true)
            {
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号           
                byte[] buffer = new byte[1024];
                int length = server.ReceiveFrom(buffer, ref point);//接收数据报  这里后面没有执行
                //创建新类
                string message = Encoding.UTF8.GetString(buffer, 0, length);
                //
                string imei = message.Substring(0, 16);
                if (!_dictionary.ContainsKey(imei))
                {
                    //不存在这个key说明是第一次接入数据
                    _dictionary.Add(imei,new DeviceInteraction()
                    {
                        Endpoint = point,
                        OnlineTime = DateTime.Now,
                        InteractionNum = 0
                    });
                    //然后发送请求报文
                    server.SendTo(Oxbuffer,_dictionary[imei].Endpoint);
                }
                else
                {
                    TimeSpan tp = DateTime.Now - _dictionary[imei].OnlineTime;
                    if (tp.Seconds > 60)
                    {
                        //说明不是本次的数据
                        _dictionary.Remove(imei);
                        _dictionary.Add(imei,new DeviceInteraction()
                        {
                            Endpoint = point,
                            OnlineTime = DateTime.Now,
                            InteractionNum = 0
                        });
                        //然后发送请求报文
                        server.SendTo(Oxbuffer,_dictionary[imei].Endpoint);
                    }
                    else
                    {
                        //说明是本次的数据
                        if (_dictionary[imei].InteractionNum == 0)
                        {
                            //第一次请求结果处理
                            byte b = buffer[19];
                            //返回报警的值
                            bool alarm = ((b & (1 << 0)) > 0) ? true : false;
                            _dictionary[imei].IsAlarm = alarm;
                            //Console.WriteLine(imei + "是否报警：" + alarm);
                            server.SendTo(Fxbuffer,_dictionary[imei].Endpoint);
                            _dictionary[imei].InteractionNum = 1;
                        }
                        else
                        {
                            //第二次请求结果处理
                            byte t = buffer[19];
                            buffer[19] = buffer[20];
                            buffer[20] = t;
                            float Temperature = ((float)(BitConverter.ToInt16(buffer, 19)) / 100 - 4) / 16 * 150;
                            t = buffer[21];
                            buffer[21] = buffer[22];
                            buffer[22] = t;
                            float OffsetValue = ((float)(BitConverter.ToInt16(buffer, 21)) / 100 - 4) / 16 * 450;
                            _dictionary[imei].Temperature = Temperature;
                            _dictionary[imei].OffsetValue = OffsetValue;
                            GetStringByHttpPostRaw(new AddDeviceDataRequestDto()
                            {
                                Imei = imei,
                                IsAlarm = _dictionary[imei].IsAlarm,
                                Temperature = _dictionary[imei].Temperature.ToString("F2"),
                                OffsetValue = _dictionary[imei].OffsetValue
                            }); ;
                            //Console.WriteLine(imei + "温度：" + Temperature + "偏移量：" + OffsetValue);
                            _dictionary.Remove(imei);
                        }
                    }
                }
            }
        }
    }
}
