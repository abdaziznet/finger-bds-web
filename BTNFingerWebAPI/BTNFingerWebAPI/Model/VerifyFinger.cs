namespace BTNFingerWebAPI.Model
{

    public class VerifyFinger
    {
        public string TellerNumber { get; set; }

        public VerifyFinger()
        {
            this.TellerNumber = string.Empty;
        }
    }

}
