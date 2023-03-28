using System.Runtime.InteropServices;

namespace ConsAppCallBTNLibrary
{
    public class Program
    {

        [DllImport("FINGER_BTN_WEB.dll")]
        public static extern int fs_init(int timeout);

        [DllImport("FINGER_BTN_WEB.dll")]
        public static extern int fs_send(int operationId, string userId);

        [DllImport("FINGER_BTN_WEB.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int fs_receive(out IntPtr respondCode, out IntPtr respondMessage);

        static void Main(string[] args)
        {

            try
            {
                Console.WriteLine("====================== Console application Simulation BDS Web using .Net Core =============================");
                int err = 0;

                Console.WriteLine("Starting call fs_init");
                err = fs_init(30);
                if (err != 0)
                {
                    Console.WriteLine("Call fs_init failed, please check log");
                    return;
                }
                Console.WriteLine("Call fs_init succeeded");
                Console.WriteLine("Call fs_init finished");

                Console.WriteLine("Please enter teller number:");

                string? telNo = string.Empty;
                telNo = Console.ReadLine();

                Console.WriteLine(string.Format("your teller number is: {0}", telNo));

                Console.WriteLine("Starting call fs_send");
                err = fs_send(1, telNo);
                if (err != 0)
                {
                    Console.WriteLine("Call fs_send failed, please check log");
                    return;
                }

                Console.WriteLine("Call fs_send succeeded");
                Console.WriteLine("Call fs_send finished");

                Console.WriteLine("Place your finger");

                Console.WriteLine("Starting call fs_receive");

                IntPtr errCode = IntPtr.Zero;
                IntPtr errMessage = IntPtr.Zero;
                err = fs_receive(out errCode, out errMessage);
                if (err != 0)
                {
                    Console.WriteLine("Call fs_receive failed, please check log");
                    return;
                }

                Console.WriteLine("Call fs_receive succeeded");
                Console.WriteLine("Call fs_receive finished");

                int iErrorCode = errCode.ToInt32();
                string strErrMessage = Marshal.PtrToStringAnsi(errMessage);

                Console.WriteLine(string.Format("Error Code: {0} | Error Messsage: {1}", iErrorCode.ToString(), strErrMessage.ToString()));

                Marshal.FreeHGlobal(errCode);
                Marshal.FreeHGlobal(errMessage);

                Console.ReadLine();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }
}