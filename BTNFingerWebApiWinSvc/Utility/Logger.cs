namespace BTNFingerWebApiWinSvc.Utility
{
	public class Logger
	{
		private readonly string logFilePath;

		public Logger()
		{
			IConfiguration configuration = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.Build();

			string logPath = configuration.GetSection("AppSettings")["LogPath"];

			this.logFilePath = Path.Combine(logPath, "BSDWEB_SERVICE_LOCAL.LOG");
		}

		public void Log(string message)
		{
			string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			string logEntry = $"{timestamp} - {message}";

			using (StreamWriter writer = new StreamWriter(logFilePath, true))
			{
				writer.WriteLine(logEntry);
			}
		}
	}
}
