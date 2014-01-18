namespace Edgebot.JSON
{
    public class JsonFishService
	{
		public int mcbans { get; set; }
		public int mcbouncer { get; set; }
		public int mcblockit { get; set; }
		public int minebans { get; set; }
		public int glizer { get; set; }
	}

    public class JsonFishStats
	{
		public string username { get; set; }
		public string uuid { get; set; }
		public int totalbans { get; set; }
        public JsonFishService service { get; set; }
	}

	public class JsonFish
	{
		public bool success { get; set; }
        public JsonFishStats stats { get; set; }
	}
}