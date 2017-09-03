using System.Collections.Generic;

namespace Hspi.HspiPlugin3.Events
{
    public class GenericEventArgs
    {
        public string Event { get; set; }

        public List<string> Parameters { get; set; }

        public string Sender { get; set; }
    }
}