using BDSWebApp.Entity;
using BDSWebApp.Framework;
using BDSWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;

namespace BDSWebApp.Controllers
{
	public class LoginController : Controller
	{
		private const string PORT = "8070";

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
				ModelState.AddModelError("", string.Format("Response code: {0} | Response message: {1}", response.ResponseCode.ToString(), response.ResponseMessage));
				return View("Login");
			}

			return View("Login");
		}

		[HttpPost]
		public IActionResult LoginSend(string dataModel)
		{
			ServiceResponse response = new ServiceResponse();

			LoginViewModel model = JsonConvert.DeserializeObject(dataModel, typeof(LoginViewModel)) as LoginViewModel;

			if (string.IsNullOrEmpty(model.TellerNumber))
			{
				return new JsonResult(new { responseCode = 999, responseMessage = "Please put Teller Number" });
			}

			response = FingerSend(model.TellerNumber);

			if (response == null)
			{
				return new JsonResult(new { responseCode = 999, responseMessage = "Response Message Null" });
			}

			if (response.ResponseCode != 0)
			{
				return new JsonResult(new { responseCode = response.ResponseCode, responseMessage = response.ResponseMessage });
			}

			var jsonResult = new { responseCode = response.ResponseCode, responseMessage = response.ResponseMessage, url = Url.Action("LoginReceive", "Login") };

			return new JsonResult(jsonResult);

		}

		public IActionResult LoginReceive()
		{
			ServiceResponse response = new ServiceResponse();
			Logger logger = new Logger();

			try
			{
				response = FingerReceive();

				if (response == null)
				{
					ModelState.AddModelError("", "Response NULL");
					return View("Login");
				}

				if (response.ResponseCode != 0)
				{
					ModelState.AddModelError("", string.Format("Response code: {0} | Response message: {1}", response.ResponseCode.ToString(), response.ResponseMessage));
					return View("Login");
				}

			}
			catch (Exception ex)
			{
				logger.Log(ex.Message);
			}

			return RedirectToAction("Index", "Home");
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
			Logger logger = new Logger();
			try
			{
				logger.Log("Start finger init web api");

				logger.Log("Start get client ip address ");

				IConfiguration configuration = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.Build();

				string mode = configuration.GetSection("AppSettings")["Mode"];

				string clientIP = string.Empty;
				if (mode.ToLower() == "local")
				{
					clientIP = "localhost";
				}
				else
				{
					clientIP = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
				}

				logger.Log(string.Format("Client ip address: {0}", clientIP));

				logger.Log("End get client ip address ");

				var handler = new TimeoutHandler
				{
					DefaultTimeout = TimeSpan.FromSeconds(Convert.ToInt32(30)),
					InnerHandler = new HttpClientHandler()
				};

				ClientInfo clientInfo = new ClientInfo();
				clientInfo.TimeOut = 30;
				var jsonBody = JsonConvert.SerializeObject(clientInfo);
				var requestBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				var url = string.Format("http://{0}:{1}/Finger/init", clientIP, PORT);
				logger.Log(string.Format("Url init: {0}", url));


				System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };

				using (var cts = new CancellationTokenSource())
				using (var client = new HttpClient(handler))
				{
					client.Timeout = Timeout.InfiniteTimeSpan;
					var request = new HttpRequestMessage(HttpMethod.Post, url);
					request.Content = requestBody;

					// Set request headers
					client.DefaultRequestHeaders.Add("accept", "*/*");

					string jsonResponse = string.Empty;
					var responseApi = client.SendAsync(request, cts.Token);
					jsonResponse = responseApi.Result.Content.ReadAsStringAsync().Result;

					if (!IsValidJson(jsonResponse))
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "Invalid JsonResponse";
						logger.Log("Invalid JsonResponse");
						return response;
					}

					var deserializeResp = new ServiceResponse();
					deserializeResp = JsonConvert.DeserializeObject<ServiceResponse>(jsonResponse);

					if (!responseApi.Result.IsSuccessStatusCode)
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "StatusCode Not Success";
						logger.Log("StatusCode Not Success");

						return response;
					}

					if (deserializeResp.ResponseCode != 0)
					{
						response.ResponseCode = 999;
						response.ResponseMessage = string.Format("Response Code: {0} | Response Message: {1}", deserializeResp.ResponseCode, deserializeResp.ResponseMessage);
						logger.Log(string.Format("Response Code: {0} | Response Message: {1}", deserializeResp.ResponseCode, deserializeResp.ResponseMessage));

						return response;
					}

					logger.Log("Finger init web api success");

				}
			}
			catch (Exception ex)
			{
				response.ResponseCode = 999;
				response.ResponseMessage = ex.Message;
				logger.Log(ex.Message);
				return response;
			}
			finally
			{
				logger.Log("End finger init web api");
			}

			return response;
		}

		private ServiceResponse FingerSend(string tellerNumber)
		{
			ServiceResponse response = new ServiceResponse();
			Logger logger = new Logger();

			try
			{
				logger.Log("Start finger send web api");

				IConfiguration configuration = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.Build();

				string mode = configuration.GetSection("AppSettings")["Mode"];

				string clientIP = string.Empty;
				if (mode.ToLower() == "local")
				{
					clientIP = "localhost";
				}
				else
				{
					clientIP = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
				}

				var handler = new TimeoutHandler
				{
					DefaultTimeout = TimeSpan.FromSeconds(Convert.ToInt32(30)),
					InnerHandler = new HttpClientHandler()
				};

				VerifyFinger verifyFinger = new VerifyFinger();
				verifyFinger.TellerNumber = tellerNumber;

				var jsonBody = JsonConvert.SerializeObject(verifyFinger);
				var requestBody = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				var url = string.Format("http://{0}:{1}/Finger/send", clientIP, PORT);
				logger.Log(string.Format("Url init: {0}", url));

				System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };

				using (var cts = new CancellationTokenSource())
				using (var client = new HttpClient(handler))
				{
					client.Timeout = Timeout.InfiniteTimeSpan;
					var request = new HttpRequestMessage(HttpMethod.Post, url);
					request.Content = requestBody;

					// Set request headers
					client.DefaultRequestHeaders.Add("accept", "*/*");

					string jsonResponse = string.Empty;
					var responseApi = client.SendAsync(request, cts.Token);
					jsonResponse = responseApi.Result.Content.ReadAsStringAsync().Result;

					if (!IsValidJson(jsonResponse))
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "Invalid JsonResponse";
						logger.Log("Invalid JsonResponse");

						return response;
					}

					var deserializeResp = new ServiceResponse();
					deserializeResp = JsonConvert.DeserializeObject<ServiceResponse>(jsonResponse);

					if (!responseApi.Result.IsSuccessStatusCode)
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "StatusCode Not Success";
						logger.Log("StatusCode Not Success");

						return response;
					}

					if (deserializeResp.ResponseCode != 0)
					{
						response.ResponseCode = deserializeResp.ResponseCode;
						response.ResponseMessage = deserializeResp.ResponseMessage;
						logger.Log(string.IsNullOrEmpty(deserializeResp.ResponseMessage) ? "OK" : deserializeResp.ResponseMessage);

						return response;
					}

					logger.Log("Finger send web api success");
				}
			}
			catch (Exception ex)
			{
				response.ResponseCode = 999;
				response.ResponseMessage = ex.Message;
				logger.Log(ex.Message);

				return response;
			}
			finally
			{
				logger.Log("End finger send web api");
			}

			return response;
		}

		private ServiceResponse FingerReceive()
		{
			ServiceResponse response = new ServiceResponse();
			Logger logger = new Logger();

			try
			{
				logger.Log("Start finger receive web api");

				IConfiguration configuration = new ConfigurationBuilder()
			.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			.Build();

				string mode = configuration.GetSection("AppSettings")["Mode"];

				string clientIP = string.Empty;
				if (mode.ToLower() == "local")
				{
					clientIP = "localhost";
				}
				else
				{
					clientIP = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
				}

				var handler = new TimeoutHandler
				{
					DefaultTimeout = TimeSpan.FromSeconds(Convert.ToInt32(30)),
					InnerHandler = new HttpClientHandler()
				};

				var url = string.Format("http://{0}:{1}/Finger/receive", clientIP, PORT);
				logger.Log(string.Format("Url init: {0}", url));

				System.Net.ServicePointManager.ServerCertificateValidationCallback = (senderX, certificate, chain, sslPolicyErrors) => { return true; };

				using (var cts = new CancellationTokenSource())
				using (var client = new HttpClient(handler))
				{
					client.Timeout = Timeout.InfiniteTimeSpan;
					var request = new HttpRequestMessage(HttpMethod.Post, url);

					// Set request headers
					client.DefaultRequestHeaders.Add("accept", "*/*");

					string jsonResponse = string.Empty;
					var responseApi = client.SendAsync(request, cts.Token);
					jsonResponse = responseApi.Result.Content.ReadAsStringAsync().Result;

					if (!IsValidJson(jsonResponse))
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "Invalid JsonResponse";
						logger.Log("Invalid JsonResponse");

						return response;
					}

					var deserializeResp = new ServiceResponse();
					deserializeResp = JsonConvert.DeserializeObject<ServiceResponse>(jsonResponse);

					if (!responseApi.Result.IsSuccessStatusCode)
					{
						response.ResponseCode = 999;
						response.ResponseMessage = "StatusCode Not Success";
						logger.Log("StatusCode Not Success");

						return response;
					}

					logger.Log("Finger receive web api success");

					if (deserializeResp.ResponseCode != 0)
					{
						response.ResponseCode = deserializeResp.ResponseCode;
						response.ResponseMessage = deserializeResp.ResponseMessage;
						logger.Log(string.Format("Response Code: {0} | Response Message: {1}", response.ResponseCode, response.ResponseMessage));

						return response;
					}

					logger.Log(string.Format("Response Code: {0} | Response Message: {1}", response.ResponseCode, response.ResponseMessage));

				}
			}
			catch (Exception ex)
			{
				response.ResponseCode = 999;
				response.ResponseMessage = ex.Message;
				logger.Log(ex.Message);

				return response;
			}
			finally
			{
				logger.Log("End finger receive web api");
			}

			return response;
		}
	}
}
