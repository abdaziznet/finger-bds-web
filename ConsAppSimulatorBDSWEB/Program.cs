using FingerBTNWebCore;

namespace ConsAppSimulatorBDSWEB
{
    public class Program
    {
        static void Main(string[] args)
        {

            try
            {
                Console.WriteLine("====================== Console application Simulation BDS Web using .Net Core =============================");
                int err = 0;
                Console.WriteLine("Starting call fs_web_init");
                err = FingerWebCore.fs_web_init(30);
                if (err != 0)
                {
                    Console.WriteLine("Call fs_web_init failed, please check log");
                    return;
                }
                Console.WriteLine("Call fs_web_init succeeded");
                Console.WriteLine("Call fs_web_init finished");

                Console.WriteLine("Please enter teller number:");

                string? tellerNumber = string.Empty;
                tellerNumber = Console.ReadLine();

                if (string.IsNullOrEmpty(tellerNumber))
                {
                    Console.WriteLine("Teller number is mandatory!");
                    return;

                }

                Console.WriteLine("Starting call fs_send");
                err = FingerWebCore.fs_web_send(1, tellerNumber);
                if (err != 0)
                {
                    Console.WriteLine("Call fs_web_send failed, please check log");
                    return;
                }

                Console.WriteLine("Call fs_web_send succeeded");
                Console.WriteLine("Call fs_web_send finished");

                Console.WriteLine("Place your finger");
                Console.WriteLine("Place your finger");
                Console.WriteLine("Place your finger");

                Console.WriteLine("Starting call fs_web_receive");

                int errCode = 0;
                string errMessage = string.Empty;
                err = FingerWebCore.fs_web_receive(out errCode, out errMessage);
                if (err != 0)
                {
                    Console.WriteLine("Call fs_web_receive failed, please check log");
                    return;
                }

                Console.WriteLine("Call fs_web_receive succeeded");
                Console.WriteLine("Call fs_web_receive finished");

                Console.WriteLine(string.Format("Error Code: {0} | Error Messsage: {1}", errCode.ToString(), errMessage.ToString()));

                Console.ReadLine();

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("BDS Web Finished");
            }
        }

    }
}