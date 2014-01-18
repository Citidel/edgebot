namespace Edgebot.JSON
{
	public class FishService
	{
		public int mcbans { get; set; }
		public int mcbouncer { get; set; }
		public int mcblockit { get; set; }
		public int minebans { get; set; }
		public int glizer { get; set; }
	}

	public class FishStats
	{
		public string username { get; set; }
		public string uuid { get; set; }
		public int totalbans { get; set; }
		public FishService service { get; set; }
	}

	public class FishRootObject
	{
		public bool success { get; set; }
		public FishStats stats { get; set; }
	}
}