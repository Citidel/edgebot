namespace Edgebot.Classes.JSON
{
    public class JsonSrvResult
    {
        public string rr1 { get; set; }
        public string rr2 { get; set; }
        public string ftb1 { get; set; }
    }
    public class JsonSrv
    {
        public JsonSrvResult result { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
    }
}