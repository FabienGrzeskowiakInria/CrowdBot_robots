
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace crowdbotsim
{
    public class CrowdTcpPublisher : TcpPublisher
    {

        [SerializeField]
        [Tooltip ("Frame id for Tf")]
        public string FrameId = "undefined";
        private string message;
        private string sensor_infos = "";
        
        private CrowdBotSim_TrialManager trialManager;
        protected override void Start()
        {
            trialManager = GameObject.FindGameObjectWithTag("Stage").GetComponent<CrowdBotSim_TrialManager>();

            base.Start();

            sensor_infos = trialManager.get_agents_list().Count.ToString();

        }

        public override string Publish(string data_id, float time)
        {
            string header = concat_to(Topic, '=', data_id);

            // publish infos on first message only
            // if(int.Parse(data_id) == 0)
            // {
            //     header = concat_to(header, '-', sensor_infos);
            // }

            return concat_to(header, '#', message);
        }

        private void LateUpdate()
        {
            UpdateMessage();
            if(!trialManager.gameObject.activeInHierarchy)
                this.Start();
        }

        public void UpdateMessage()
        {
            if(ToolsTime.DeltaTime > 0)
            {
                message = "id pose";

                List<Agent> agents = trialManager.get_agents_list();
                foreach (var agent in agents)
                {
                    message = concat_to(message, ' ', agent.GetID().ToString());
                    message = concat_to(message, ' ', '(' + agent.Position.x.ToString("F3") + ' ' + agent.Position.z.ToString("F3") + ')');
                }
            }
        }

    }
}