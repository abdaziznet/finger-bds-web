using GenHTTP.Api.Protocol;
using GenHTTP.Modules.Webservices;
using FingerBTNWebCore;
using BTNFingerWebAPI.Model;

namespace BTNFingerWebAPI.Services
{
    public class FingerService
    {
        [ResourceMethod(RequestMethod.POST)]
        public FingerResult BdsWebLogin(VerifyFinger model)
        {
            //http://localhost:8080/bdsweblogin

            var result = new FingerResult();

            int err;
            err = FingerWebCore.fs_web_init(30);
            if (err != 0)
            {
                result.ResponseCode = err;
                result.ResponseMessage = "Call fs init failed!";
                return result;
            }

            if (string.IsNullOrWhiteSpace(model.TellerNumber))
            {
                result.ResponseCode = 9;
                result.ResponseMessage = "Teller Number is mandatory";
                return result;

            }

            err = FingerWebCore.fs_web_send(1, model.TellerNumber);
            if (err != 0)
            {
                result.ResponseCode = err;
                result.ResponseMessage = "Call fs send failed!";
                return result;
            }

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



        

        


        //[ResourceMethod(RequestMethod.POST)]
        //public HttpResponseMessage BdsLogin(VerifyFinger model)
        //{
        //    //http://localhost:8080/bdslogin

        //    var response = new HttpResponseMessage();

        //    int err;
        //    err = FingerWebCore.fs_web_init(30);
        //    if (err != 0)
        //    {
        //        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //        response.Content = new StringContent("Call fs_web_init failed!");
        //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        //        return response;
        //    }

        //    if (string.IsNullOrWhiteSpace(model.TellerNumber))
        //    {
        //        response = new HttpResponseMessage(HttpStatusCode.BadRequest);
        //        return response;

        //    }

        //    err = FingerWebCore.fs_web_send(1, model.TellerNumber);
        //    if (err != 0)
        //    {
        //        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //        response.Content = new StringContent("Call fs_web_send failed!");
        //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        //        return response;
        //    }

        //    int errCode = 0;
        //    string errMessage = string.Empty;
        //    err = FingerWebCore.fs_web_receive(out errCode, out errMessage);
        //    if (err != 0)
        //    {
        //        response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
        //        response.Content = new StringContent("Call fs_web_receive failed!");
        //        response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        //        return response;
        //    }

        //    response = new HttpResponseMessage(HttpStatusCode.OK);
        //    response.Content = new StringContent(string.Format("Error Code: {0} | Error Messsage: {1}", errCode.ToString(), errMessage.ToString()));
        //    response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
        //    return response;

        //}



    }
}