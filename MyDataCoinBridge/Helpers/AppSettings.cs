using System;
namespace MyDataCoinBridge.Helpers
{
	public class AppSettings
	{
		public string DB_CONNECTION { get; set; }
		public string JWT_KEY { get; set; }
		public string G_API_KEY { get; set; }
		public string G_API_PASSWORD { get; set; }
		public string G_IMAGE_TOKEN { get; set; }
		public string FCM_CONFIGURATION { get; set; }
		public string BRIDGE_URI { get; set; }
	}
}

