
namespace BTNFingerWebAPI.Model
{
    public class FingerResult
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }

        public FingerResult() { 
            this.ResponseCode = 0;
            this.ResponseMessage = "";
        }
    }
}
