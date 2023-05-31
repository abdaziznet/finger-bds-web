using BTNFingerWebApiWinSvc.Model;
using BTNFingerWebApiWinSvc.Utility;
using FingerBTNWebCore;
using Microsoft.AspNetCore.Mvc;

namespace BTNFingerWebApiWinSvc.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class FingerController : ControllerBase
	{
		[HttpPost]
		[Route("init")]
		public IActionResult Init([FromBody] ClientInfo clientinfo)
		{
			FingerResponse response = new FingerResponse();
			Logger logger = new Logger();

			try
			{
				int err = int.MaxValue;
				logger.Log("Start fs_web_init");
				err = FingerWebCore.fs_web_init(clientinfo.Timeout);
				if (err != 0)
				{
					response.ResponseCode = err;
					response.ResponseMessage = "Call fs init failed!";
					logger.Log(string.Format("error code: {0} | message: {1}", response.ResponseCode, response.ResponseMessage));
					return BadRequest(response);
				}

				response.ResponseCode = 0;
				response.ResponseMessage = "Finger init success";
				logger.Log(string.Format("error code: {0} | message: {1}", response.ResponseCode, response.ResponseMessage));

			}
			catch (Exception ex)
			{
				response.ResponseCode = 999;
				response.ResponseMessage = ex.Message;
				logger.Log(string.Format("error code: {0} | message: {1}", response.ResponseCode, response.ResponseMessage));
				return StatusCode(500, "An internal server error occurred.");
			}
			finally
			{
				logger.Log("End fs_web_init");
			}

			return Ok(response);
		}

		[HttpPost]
		[Route("send")]
		public IActionResult Send([FromBody] VerifyFinger verify)
		{
			FingerResponse response = new FingerResponse();
			Logger logger = new Logger();

			try
			{
				int err = int.MaxValue;
				logger.Log("Start fs_web_send");
				err = FingerWebCore.fs_web_send(verify.OperationId, verify.TellerNumber);
				if (err != 0)
				{
					response.ResponseCode = err;
					response.ResponseMessage = "Call fs send failed!";
					logger.Log(string.Format("error code: {0} | message: {1}", response.ResponseCode, response.ResponseMessage));
					return BadRequest(response);
				}

				response.ResponseCode = 0;
				response.ResponseMessage = "Finger send success";
				logger.Log(string.Format("error code: {0} | message: {1}", response.ResponseCode, response.ResponseMessage));

			}
			catch (Exception ex)
			{
				response.ResponseCode = 999;
				response.ResponseMessage = ex.Message;
				logger.Log(string.Format("error code: {0} | message: {1}", response.ResponseCode, response.ResponseMessage));
				return StatusCode(500, "An internal server error occurred.");
			}
			finally
			{
				logger.Log("End fs_web_send");
			}

			return Ok(response);
		}

		[HttpPost]
		[Route("receive")]
		public IActionResult Receive()
		{
			FingerResponse response = new FingerResponse();
			Logger logger = new Logger();

			try
			{
				int err = int.MaxValue;
				logger.Log("Start fs_web_receive");

				int errCode = 0;
				string message = string.Empty;
				err = FingerWebCore.fs_web_receive(out errCode, out message);
				if (err != 0)
				{
					response.ResponseCode = err;
					response.ResponseMessage = "Call fs receive failed!";
					logger.Log(string.Format("error code: {0} | message: {1}", response.ResponseCode, response.ResponseMessage));
					return BadRequest(response);
				}

				response.ResponseCode = errCode;
				response.ResponseMessage = message.ToString();
				logger.Log(string.Format("error code: {0} | message: {1}", response.ResponseCode, response.ResponseMessage));

			}
			catch (Exception ex)
			{
				response.ResponseCode = 999;
				response.ResponseMessage = ex.Message;
				logger.Log(string.Format("error code: {0} | message: {1}", response.ResponseCode, response.ResponseMessage));
				return StatusCode(500, "An internal server error occurred.");
			}
			finally
			{
				logger.Log("End fs_web_receive");
			}

			return Ok(response);
		}
	}
}
