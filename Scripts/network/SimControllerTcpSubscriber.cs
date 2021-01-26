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
