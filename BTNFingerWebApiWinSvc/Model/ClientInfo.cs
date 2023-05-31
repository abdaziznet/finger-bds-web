namespace BTNFingerWebApiWinSvc.Model
{
	public class ClientInfo
	{
		public int Timeout { get; set; }

		public ClientInfo()
		{
			this.Timeout = 30;
		}
	}
}
