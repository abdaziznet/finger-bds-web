using BTNFingerWebAPI.Model;
using FingerBTNWebCore;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Webservices;

namespace BTNFingerWebAPI.Services
{
    public class FingerInitService
    {
        [ResourceMethod(RequestMethod.POST)]
        public FingerResult FingerInit(ClientInfo clientInfo)
        {
            var result = new FingerResult();
            int err;
            err = FingerWebCore.fs_web_init(clientInfo.Timeout);
            if (err != 0)
            {
                result.ResponseCode = err;
                result.ResponseMessage = "Call fs init failed!";
                return result;
            }

            result.ResponseCode = 0;
            result.ResponseMessage = "Finger init success";
            return result;
        }
    }
}
