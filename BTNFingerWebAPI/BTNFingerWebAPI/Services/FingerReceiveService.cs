using BTNFingerWebAPI.Model;
using FingerBTNWebCore;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Webservices;

namespace BTNFingerWebAPI.Services
{
    public class FingerReceiveService
    {
        [ResourceMethod(RequestMethod.POST)]
        public FingerResult FingerReceive()
        {
            var result = new FingerResult();
            int err;

            int errCode = 0;
            string errMessage = string.Empty;
            err = FingerWebCore.fs_web_receive(out errCode, out errMessage);
            if (err != 0)
            {
                result.ResponseCode = err;
                result.ResponseMessage = "Call fs receive failed!";
                return result;
            }

            result.ResponseCode = err;
            result.ResponseMessage = errMessage.ToString();
            return result;
        }
    }
}
