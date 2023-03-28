using System.Runtime.InteropServices;

namespace FingerBTNWebCore;
public static class FingerWebCore
{
    [DllImport("FINGER_BTN_WEB.dll")]
    private static extern int fs_init(int timeout);

    [DllImport("FINGER_BTN_WEB.dll")]
    private static extern int fs_send(int operationId, string userId);

    [DllImport("FINGER_BTN_WEB.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
    private static extern int fs_receive(out IntPtr respondCode, out IntPtr respondMessage);

    public static int fs_web_init(int timeout)
    {
        int err;
        err = fs_init(timeout);
        return err;
    }

    public static int fs_web_send(int operationId, string userId)
    {
        int err;
        err = fs_send(operationId, userId);
        return err;
    }

    public static int fs_web_receive(out int respondCode, out string respondMessage)
    {
        int err;
        IntPtr errCode = IntPtr.Zero;
        IntPtr errMessage = IntPtr.Zero;

        err = fs_receive(out errCode, out errMessage);

        if (errMessage == IntPtr.Zero){
            respondCode = 998;
            respondMessage = "ERORR";
            return 1;
        }

        int iErrorCode = errCode.ToInt32();
        string strErrMessage = Marshal.PtrToStringAnsi(errMessage);

        if (string.IsNullOrEmpty(strErrMessage)){
            respondCode = 999;
            respondMessage = "ERORR";
            return 1;
        }

        //string source = "BTN";
        //string log = strErrMessage;
        //if (!EventLog.SourceExists(source))
        //{
        //    EventLog.CreateEventSource(source, log);
        //}

        respondCode = iErrorCode;
        respondMessage = strErrMessage;

        //Marshal.FreeHGlobal(errCode);
        //Marshal.FreeHGlobal(errMessage);

        return err;
    }
}
