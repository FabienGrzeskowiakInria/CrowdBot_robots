// # MIT License

// # Copyright (C) 2018-2021  CrowdBot H2020 European project
// # Inria Rennes Bretagne Atlantique - Rainbow - Julien Pettré

// # Permission is hereby granted, free of charge, to any person obtaining
// # a copy of this software and associated documentation files (the
// # "Software"), to deal in the Software without restriction, including
// # without limitation the rights to use, copy, modify, merge, publish,
// # distribute, sublicense, and/or sell copies of the Software, and to
// # permit persons to whom the Software is furnished to do so, subject
// # to the following conditions:

// # The above copyright notice and this permission notice shall be
// # included in all copies or substantial portions of the Software.

// # THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// # EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// # OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// # NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// # LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// # ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// # CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
using System;
using System.Threading;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace crowdbotsim
{
    public class TcpConnector : MonoBehaviour
    {
        public enum Protocols {TCP};
        public Protocols Protocol;
        private ManualResetEvent isConnected = new ManualResetEvent(false);
        private MonoBehaviour[] scripts; 

        [HideInInspector]
        public bool start_scripts = true;

        //TCP attributes
        [Header("Sockets basic attributes")]
        public string connectionIP = "127.0.0.1";
        public int connectionPort = 25001;
        IPAddress localAdd;
        TcpListener listener;
        TcpClient client;
        private bool running;
        private bool quit = false;

        private Dictionary<string, Delegate> SubscriberHandlers;
        private List<TcpPublisher> Publishers;

        private int data_counter = 0;

        public void Awake()
        {
            SubscriberHandlers = new Dictionary<string, Delegate>();
            Publishers = new List<TcpPublisher>();
            var main_thread = new Thread(ConnectAndWait);
            // main_thread.Priority = System.Threading.ThreadPriority.Highest;
            main_thread.Start();
        }

        private void ConnectAndWait()
        {
            if(Protocol == Protocols.TCP)
            {
                GetInfo();
            }
            
        }
        
        private void Update()
        {
            if(quit) ToolsDebug.Quit();

            if(start_scripts)
            {
                scripts = GetComponents<MonoBehaviour>();
                foreach( MonoBehaviour script in scripts )
                {
                    script.enabled = true;
                }
                start_scripts = false;
            }
        }

        void GetInfo()
        {
            localAdd = IPAddress.Parse(connectionIP);
            listener = new TcpListener(IPAddress.Any, connectionPort);
            listener.Start();

            client = listener.AcceptTcpClient();
    
            running = true;

            Debug.Log("Connected to python client");

            while (running)
            {
                TCPConnection();
            }
            listener.Stop();
            quit = true;
        }

        void TCPConnection()
        {
            NetworkStream nwStream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            int bytesRead = nwStream.Read(buffer, 0, client.ReceiveBufferSize);
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            if (dataReceived != null)
            {
                if (dataReceived == "stop")
                {
                    Debug.Log("Stop TCP server & CrowdBotSim");
                    byte[] buffer_out = Encoding.UTF8.GetBytes(dataReceived+"@");
                    nwStream.Write(buffer_out, 0, buffer_out.Length);
                    running = false;
                }
                else
                {
                    List<string> data_out = new List<string>();

                    if(dataReceived != "")
                    {
                        // put the string of incoming data into dictionnary if the incoming data are in the form of key1=value1;key2=value2 
                        //(key and value are stored as string)
                        var incoming_data = 
                        dataReceived.Split(new [] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(part => part.Split('=')).Where(part => part.Length == 2).ToDictionary(sp => sp[0], sp => sp[1]);

                        foreach( KeyValuePair<string, Delegate> sub in SubscriberHandlers )
                        {
                            string msg = null;
                            
                            incoming_data.TryGetValue(sub.Key, out msg);
                            
                            if(msg != null) 
                            {
                                sub.Value.DynamicInvoke(msg);
                            }
                        }

                        //sending clock back first
                        data_out.Add( "clock=" + data_counter.ToString() + "#" + incoming_data["clock"] );

                        foreach( TcpPublisher pub in Publishers)
                        {
                            data_out.Add(pub.Publish(data_counter.ToString(), float.Parse(incoming_data["clock"] , CultureInfo.InvariantCulture.NumberFormat) ) );
                        }


                        data_counter++;

                    }

                    data_out.Add("@"); //end
                    string join_data_out = string.Join( ";", data_out);                    
                    byte[] buffer_out = Encoding.UTF8.GetBytes(join_data_out);

                    nwStream.Write(buffer_out, 0, buffer_out.Length);
                }
            }
        }

        public void Subscribe(string Topic, Action<string> subscriberHandler)
        {        
            SubscriberHandlers[Topic] = new Action<string>(subscriberHandler);
        }

        public void Advertise(TcpPublisher pub)
        {
            Publishers.Add(pub); 
        }
    }
}