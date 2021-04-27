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

namespace crowdbotsim
{
    public class SimControllerTcpSubscriber : TcpSubscriber
    {
        private CrowdBotSim_MainManager mainManager;

        private bool first_message = true;

		protected override void Start()
		{
			base.Start();
            mainManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CrowdBotSim_MainManager>();
		}
		
        protected override void ReceiveMessage(string message)
        {
            char msg = message[0];
            switch (msg)
            {
                case 'i':
                    mainManager.state = CrowdBotSim_MainManager.MainManagerState.idle;
                break;                
                case 'n':
                    mainManager.state = CrowdBotSim_MainManager.MainManagerState.next;
                break;                
                case 'p':
                    mainManager.state = CrowdBotSim_MainManager.MainManagerState.previous;
                break;                
                case 'r':
                    mainManager.state = CrowdBotSim_MainManager.MainManagerState.reset;
                break;                
                case 'f':
                    mainManager.state = CrowdBotSim_MainManager.MainManagerState.first;
                break;                
                case 'l':
                    mainManager.state = CrowdBotSim_MainManager.MainManagerState.last;
                break;                
                case 's':
                    mainManager.state = CrowdBotSim_MainManager.MainManagerState.stop;
                break;                
                
                default:
                    ToolsDebug.logWarning("Sim controller TCP sub: Bad message"); 
                break;                
            }
        }
    }

}
