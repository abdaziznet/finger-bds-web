namespace BDSWebApp.Entity
{
    public class ServiceResponse
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }

        public ServiceResponse()
        {
            this.ResponseCode = 0;
            this.ResponseMessage = string.Empty;
        }
    }
}
