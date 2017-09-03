namespace Hspi.HspiPlugin3.Events
{
    public class SpeakerConnectEventArgs
    {
        public string IpAddress { get; set; }

        public SpeakerConnectionStatus ConnectionStatus { get; set; }

        public string Instance { get; set; }

        public string Host { get; set; }
    }
}