namespace Hspi.HspiPlugin3.Events
{
    public class ValueChangeEventArgs
    {
        public int DeviceReferenceNumber { get; set; }

        public double OldValue { get; set; }

        public double NewValue { get; set; }

        public string Address { get; set; }
    }
}