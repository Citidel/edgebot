namespace Edgebot.Classes.Instances
{
    class MojangStatus
    {
        public bool Website { get; set; }
        public bool Login { get; set; }
        public bool Session { get; set; }
        public bool Account { get; set; }
        public bool Authentication { get; set; }

        public string ToStatusString()
        {
            return Website + " " + Login + " " + Session + " " + Account + " " + Authentication;
        }
    }
}
