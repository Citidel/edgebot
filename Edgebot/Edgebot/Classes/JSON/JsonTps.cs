namespace EdgeBot.Classes.JSON
{
    public abstract class JsonTps
    {
        public string Server { get; set; }
        public float Tps { get; set; }
        public int Count { get; set; }
        public string Name { get; set; }
    }
}
