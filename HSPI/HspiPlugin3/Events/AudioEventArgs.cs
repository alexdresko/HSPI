namespace Hspi.HspiPlugin3.Events
{
    public class AudioEventArgs
    {
        public bool Started { get; set; }

        public int Devices { get; set; }

        public AudioType AudioType { get; set; }

        public string Host { get; set; }

        public string Instance { get; set; }
    }
}