using BDSWebApp.Entity;
using BDSWebApp.Framework;
using BDSWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace BDSWebApp.Controllers
{
	public class LoginController : Controller
	{
		[HttpGet]
		public IActionResult Index()
		{
			ServiceResponse response = new ServiceResponse();
			response = FingerInit();
			if (response == null)
			{
				ErrorViewModel errorModel = new ErrorViewModel();
				errorModel.RequestId = Guid.NewGuid().ToString();
				return View("Error");
			}

			if (response.ResponseCode != 0)
			{
				ErrorViewModel errorModel = new ErrorViewModel();
				errorModel.RequestId = Guid.NewGuid().ToString();
				return View("Error");
			}

			return View("Login");
		}

		[HttpPost]
		public IActionResult LoginSend(string dataModel)
		{
			ServiceResponse response = new ServiceResponse();

			if (!ModelState.IsValid)
			{
				ModelState.AddModelError("", "Invalid Teller Number");
				return View();
			}

			LoginViewModel model = JsonConvert.DeserializeObject(dataModel, typeof(LoginViewModel)) as LoginViewModel;

			response = FingerSend(model.TellerNumber);

			if (response == null)
			{
				ErrorViewModel errorModel = new ErrorViewModel();
				errorModel.RequestId = Guid.NewGuid().ToString();
				return View("Error");
			}

			if (response.ResponseCode != 0)
			{
				ModelState.AddModelError("", string.Format("Response Code: {0} | Response Message: {1}", response.ResponseCode, response.ResponseMessage));
				return View();
			}

			var jsonResult = new { responseCode = response.ResponseCode, responseMessage = response.ResponseMessage, url = Url.Action("LoginReceive", "Login") };

			return new JsonResult(jsonResult);

		}

		public IActionResult LoginReceive()
		{
			ServiceResponse response = new ServiceResponse();

			response = FingerReceive();

			if (response == null)
			{
				ErrorViewModel errorModel = new ErrorViewModel();
				errorModel.RequestId = Guid.NewGuid().ToString();
				return View("Error");
			}

			if (response.ResponseCode != 0)
			{
				ModelState.AddModelError("", string.Format("Response Code: {0} | Response Message: {1}", response.ResponseCode, response.ResponseMessage));
				return View("Login");
			}


			return RedirectToAction("Index","Home");
		}

		public bool IsValidJson(string strInput)
		{
			if (string.IsNullOrWhiteSpace(strInput)) { return false; }
			strInput = strInput.Trim();
			if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
				(strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
			{
				try
				{
					var obj = JToken.Parse(strInput);
					return true;
				}
				catch (JsonReaderException jex)
				{
					//Exception in parsing json
					Console.WriteLine(jex.Message);
					return false;
				}
				catch (Exception ex) //some other exception
				{
					Console.WriteLine(ex.ToString());
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		private ServiceResponse FingerInit()
		{
			ServiceResponse response = new ServiceResponse();

			try
			{
				var handler = new TimeoutHandler
				{
					DefaultTimeout = TimeSpan.FromSeconds(Convert.ToInt32(30)),
					InnerHandler = new HttpClientHandler()
				};

				ClientInfo clientInfo = new ClientInfo();
				clientInfo.TimeOut = 30;
				var jsonBody = JsonConvert.SerializeObject(clientInfo);
				var requestBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				var url = @"http://localhost:8080/init";

				System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };

				using (var cts = new CancellationTokenSource())
				using (var client = new HttpClient(handler))
				{
					client.Timeout = Timeout.InfiniteTimeSpan;
					var request = new HttpRequestMessage(HttpMethod.Post, url);
					request.Content = requestBody;

					string jsonResponse = string.Empty;
					var responseApi = client.SendAsync(request, cts.Token);
					jsonResponse = responseApi.Result.Content.ReadAsStringAsync().Result;

					if (!IsValidJson(jsonResponse))
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "Invalid JsonResponse";
						return response;
					}

					var deserializeResp = new ServiceResponse();
					deserializeResp = JsonConvert.DeserializeObject<ServiceResponse>(jsonResponse);

					if (!responseApi.Result.IsSuccessStatusCode)
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "StatusCode Not Success";
						return response;
					}

					if (deserializeResp.ResponseCode != 0)
					{
						response.ResponseCode = 999;
						response.ResponseMessage = string.Format("Response Code: {0} | Response Message: {1}", deserializeResp.ResponseCode, deserializeResp.ResponseMessage);
						return response;
					}

				}
			}
			catch (Exception ex)
			{
				response.ResponseCode = 999;
				response.ResponseMessage = ex.Message;
				return response;
			}

			return response;
		}

		private ServiceResponse FingerSend(string tellerNumber)
		{
			ServiceResponse response = new ServiceResponse();

			try
			{
				var handler = new TimeoutHandler
				{
					DefaultTimeout = TimeSpan.FromSeconds(Convert.ToInt32(30)),
					InnerHandler = new HttpClientHandler()
				};

				VerifyFinger verifyFinger = new VerifyFinger();
				verifyFinger.TellerNumber = tellerNumber;

				var jsonBody = JsonConvert.SerializeObject(verifyFinger);
				var requestBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				var url = @"http://localhost:8080/send";

				System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };

				using (var cts = new CancellationTokenSource())
				using (var client = new HttpClient(handler))
				{
					client.Timeout = Timeout.InfiniteTimeSpan;
					var request = new HttpRequestMessage(HttpMethod.Post, url);
					request.Content = requestBody;

					string jsonResponse = string.Empty;
					var responseApi = client.SendAsync(request, cts.Token);
					jsonResponse = responseApi.Result.Content.ReadAsStringAsync().Result;

					if (!IsValidJson(jsonResponse))
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "Invalid JsonResponse";
						return response;
					}

					var deserializeResp = new ServiceResponse();
					deserializeResp = JsonConvert.DeserializeObject<ServiceResponse>(jsonResponse);

					if (!responseApi.Result.IsSuccessStatusCode)
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "StatusCode Not Success";
						return response;
					}

					if (deserializeResp.ResponseCode != 0)
					{
						response.ResponseCode = deserializeResp.ResponseCode;
						response.ResponseMessage = deserializeResp.ResponseMessage;
						return response;
					}

				}
			}
			catch (Exception ex)
			{
				response.ResponseCode = 999;
				response.ResponseMessage = ex.Message;
				return response;
			}

			return response;
		}

		private ServiceResponse FingerReceive()
		{
			ServiceResponse response = new ServiceResponse();

			try
			{
				var handler = new TimeoutHandler
				{
					DefaultTimeout = TimeSpan.FromSeconds(Convert.ToInt32(30)),
					InnerHandler = new HttpClientHandler()
				};

				var url = @"http://localhost:8080/receive";

				System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };

				using (var cts = new CancellationTokenSource())
				using (var client = new HttpClient(handler))
				{
					client.Timeout = Timeout.InfiniteTimeSpan;
					var request = new HttpRequestMessage(HttpMethod.Post, url);

					string jsonResponse = string.Empty;
					var responseApi = client.SendAsync(request, cts.Token);
					jsonResponse = responseApi.Result.Content.ReadAsStringAsync().Result;

					if (!IsValidJson(jsonResponse))
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "Invalid JsonResponse";
						return response;
					}

					var deserializeResp = new ServiceResponse();
					deserializeResp = JsonConvert.DeserializeObject<ServiceResponse>(jsonResponse);

					if (!responseApi.Result.IsSuccessStatusCode)
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "StatusCode Not Success";
						return response;
					}

					if (deserializeResp.ResponseCode != 0)
					{
						response.ResponseCode = deserializeResp.ResponseCode;
						response.ResponseMessage = deserializeResp.ResponseMessage;
						return response;
					}

				}
			}
			catch (Exception ex)
			{
				response.ResponseCode = 999;
				response.ResponseMessage = ex.Message;
				return response;
			}

			return response;
		}


		//if (ModelState.IsValid)
		//{
		//    FingerResultViewModel fingerResultViewModel = new FingerResultViewModel();
		//    try
		//    {
		//        var handlerSend = new TimeoutHandler
		//        {
		//            DefaultTimeout = TimeSpan.FromSeconds(Convert.ToInt32(30)),
		//            InnerHandler = new HttpClientHandler()
		//        };

		//        VerifyFinger verifyFinger = new VerifyFinger();
		//        verifyFinger.TellerNumber = model.TellerNumber;


		//        var jsonBody = JsonConvert.SerializeObject(verifyFinger);
		//        var requestBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");
		//        //string rawUrl = @"http://localhost:8080/send";
		//        //if (rawUrl.Substring(rawUrl.Length - 1) == "/")
		//        //{
		//        //    rawUrl = rawUrl.Remove(rawUrl.Length - 1);
		//        //}
		//        var urlSend = @"http://localhost:8080/send";
		//        var urlReceive = @"http://localhost:8080/receive";


		//        System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };

		//        using (var ctsSend = new CancellationTokenSource())
		//        using (var clientSend = new HttpClient(handlerSend))
		//        {
		//            clientSend.Timeout = Timeout.InfiniteTimeSpan;
		//            var requestSend = new HttpRequestMessage(HttpMethod.Post, urlSend);
		//            requestSend.Content = requestBody;

		//            string jsonResponseSend = string.Empty;
		//            var response = clientSend.SendAsync(requestSend, ctsSend.Token);
		//            jsonResponseSend = response.Result.Content.ReadAsStringAsync().Result;

		//            if (!IsValidJson(jsonResponseSend))
		//            {
		//                ModelState.AddModelError("", "Invalid JsonResponse");
		//                return View();
		//            }

		//            var deserializeRespSend = new ServiceResponse();
		//            deserializeRespSend = JsonConvert.DeserializeObject<ServiceResponse>(jsonResponseSend);

		//            if (!response.Result.IsSuccessStatusCode)
		//            {
		//                ModelState.AddModelError("", response.Result.StatusCode.ToString());
		//                return View();
		//            }

		//            if (deserializeRespSend.ResponseCode != 0)
		//            {

		//                ModelState.AddModelError("", string.Format("Response Code: {0} | Response Message: {1}", deserializeRespSend.ResponseCode, deserializeRespSend.ResponseMessage));
		//                return View();
		//            }

		//        }

		//        var handlerReceive = new TimeoutHandler
		//        {
		//            DefaultTimeout = TimeSpan.FromSeconds(Convert.ToInt32(30)),
		//            InnerHandler = new HttpClientHandler()
		//        };

		//        using (var ctsReceive = new CancellationTokenSource())
		//        using (var clientReceive = new HttpClient(handlerReceive))
		//        {
		//            clientReceive.Timeout = Timeout.InfiniteTimeSpan;
		//            var request = new HttpRequestMessage(HttpMethod.Post, urlReceive);

		//            string jsonResponseReceive = string.Empty;
		//            var response = clientReceive.SendAsync(request, ctsReceive.Token);
		//            jsonResponseReceive = response.Result.Content.ReadAsStringAsync().Result;

		//            if (!IsValidJson(jsonResponseReceive))
		//            {
		//                ModelState.AddModelError("", "Invalid JsonResponse");
		//                return View();
		//            }

		//            var deserializeRespReceive = new ServiceResponse();
		//            deserializeRespReceive = JsonConvert.DeserializeObject<ServiceResponse>(jsonResponseReceive);

		//            if (!response.Result.IsSuccessStatusCode)
		//            {
		//                ModelState.AddModelError("", response.Result.StatusCode.ToString());
		//                return View();
		//            }

		//            if (deserializeRespReceive.ResponseCode != 0)
		//            {

		//                ModelState.AddModelError("", string.Format("Response Code: {0} | Response Message: {1}", deserializeRespReceive.ResponseCode, deserializeRespReceive.ResponseMessage));
		//                return View();
		//            }

		//        }


		//    }
		//    catch (Exception ex)
		//    {
		//        ModelState.AddModelError("", ex.Message);
		//        return View();
		//    }

		//}
		//else
		//{
		//    ModelState.AddModelError("", "Invalid Teller Number");
		//    return View();
		//}
	}
}
