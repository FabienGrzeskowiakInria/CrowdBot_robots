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
using UnityEngine;
using RosSharp.RosBridgeClient.Messages.crowdbotsim;
using RosSharp.RosBridgeClient.Messages;
using System.Linq;

namespace crowdbotsim
{
    [RequireComponent(typeof(TcpConnector))]
    public abstract class TcpPublisher : MonoBehaviour
    {
        public string Topic;

        protected virtual void Start()
        {
            GetComponent<TcpConnector>().Advertise(this);
        }

        public abstract string Publish(string id="-1", float time = -1);
        public static string concat_to(string a, char sep, float[] b)
        {
            return a + sep + string.Join(" ", b.Select(f => f.ToString("0.000"))) ;
        }
        public static string concat_to(string a, char sep, int[] b)
        {
            return a + sep + string.Join(" ", b.Select(i => i.ToString())) ;
        }
        public static string concat_to(string a, char sep, float b)
        {
            return a + sep + b.ToString("0.000");
        }
        public static string concat_to(string a, char sep, string b)
        {
            return a + sep + b;
        }

    }
}