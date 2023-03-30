using Microsoft.AspNetCore.Mvc;
using SampleWebAPI.Model;
using FingerBTNWebCore;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SampleWebAPI.Controllers
{
    [ApiController]
    public class FingerController : ControllerBase
    {
        [HttpPost]
        [Route("finger/bdsweblogin")]
        public IActionResult BDSWebLogin(VerifyFinger model)
        {
            FingerResult result = new FingerResult();

            int err;
            err = FingerWebCore.fs_web_init(30);
            if (err != 0)
            {
                result.ResponseCode = err;
                result.ResponseMessage = "ERROR";
                return BadRequest(result);
            }

            err = FingerWebCore.fs_web_send(1, model.TellerNumber);
            if (err != 0)
            {
                result.ResponseCode = err;
                result.ResponseMessage = "ERROR";
                return BadRequest(result);
            }

            int errCode = 0;
            string errMessage = string.Empty;
            err = FingerWebCore.fs_web_receive(out errCode, out errMessage);
            if (err != 0)
            {
                result.ResponseCode = err;
                result.ResponseMessage = "ERROR";
                return BadRequest(result);
            }

            result.ResponseCode = errCode;
            result.ResponseMessage = errMessage;
            return Ok(result);
        }

        [HttpPost]
        [Route("finger/bdswebapprove")]
        public IActionResult BDSWebApprove(VerifyFinger model)
        {
            FingerResult result = new FingerResult();

            int err;
            err = FingerWebCore.fs_web_init(30);
            if (err != 0)
            {
                result.ResponseCode = err;
                result.ResponseMessage = "ERROR";
                return BadRequest(result);
            }

            err = FingerWebCore.fs_web_send(2, model.TellerNumber);
            if (err != 0)
            {
                result.ResponseCode = err;
                result.ResponseMessage = "ERROR";
                return BadRequest(result);
            }

            int errCode = 0;
            string errMessage = string.Empty;
            err = FingerWebCore.fs_web_receive(out errCode, out errMessage);
            if (err != 0)
            {
                result.ResponseCode = err;
                result.ResponseMessage = "ERROR";
                return BadRequest(result);
            }
            
            result.ResponseCode = errCode;
            result.ResponseMessage = errMessage;
            return Ok(result);
        }
    }
}
