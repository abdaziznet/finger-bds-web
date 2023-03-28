using BTNFingerWebAPI.Model;
using FingerBTNWebCore;
using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Webservices;


namespace BTNFingerWebAPI.Services
{
    public class FingerSendService
    {
        [ResourceMethod(RequestMethod.POST)]
        public FingerResult FingerSend(VerifyFinger verifyFinger)
        {
            var result = new FingerResult();
            int err;

            if (string.IsNullOrWhiteSpace(verifyFinger.TellerNumber))
            {
                result.ResponseCode = 9;
                result.ResponseMessage = "Teller Number is mandatory";
                return result;

            }

            err = FingerWebCore.fs_web_send(1, verifyFinger.TellerNumber);
            if (err != 0)
            {
                result.ResponseCode = err;
                result.ResponseMessage = "Call fs send failed!";
                return result;
            }

            result.ResponseCode = 0;
            result.ResponseMessage = "Finger send success";
            return result;
        }
    }
}
