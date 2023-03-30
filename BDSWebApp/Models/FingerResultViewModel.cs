namespace BDSWebApp.Models
{
    public class FingerResultViewModel
    {
        public int ResponseCode { get; set; }
        public string ResponseMessag { get; set; }

        public FingerResultViewModel()
        {
            this.ResponseCode = 0;
            this.ResponseMessag = string.Empty;
        }
    }
}
