namespace BTNFingerWebApiWinSvc.Model
{
	public class VerifyFinger
	{
		public string TellerNumber { get; set; }
		public int OperationId { get; set; }

		public VerifyFinger()
		{
			this.TellerNumber = string.Empty;
			this.OperationId = 1;
		}
	}
}
