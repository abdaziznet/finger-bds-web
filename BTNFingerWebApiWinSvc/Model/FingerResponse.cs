namespace BTNFingerWebApiWinSvc.Model
{
	public class FingerResponse
	{
		public int ResponseCode { get; set; }
		public string ResponseMessage { get; set; }

		public FingerResponse()
		{
			this.ResponseCode = 0;
			this.ResponseMessage = string.Empty;
		}
	}
}
