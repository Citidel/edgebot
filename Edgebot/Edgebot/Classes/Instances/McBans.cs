using System.Collections.Generic;

namespace EdgeBot.Classes.Instances
{
    class McBans
    {
        public int Total { get; set; }
        public double Reputation { get; set; }
        public float Pid { get; set; }
        public List<string> Global { get; set; }
        public List<string> Local { get; set; }
        public List<string> Other { get; set; }
    }
}
