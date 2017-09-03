namespace Hspi.HspiPlugin3.Events
{
    public class CallerIdEventArgs
    {
        public string Line { get; set; }

        public string AddressBookCompany { get; set; }

        public string AddressBookFirstName { get; set; }

        public string AddressBookLastName { get; set; }

        public string PhoneCompanyName { get; set; }

        public string Number { get; set; }
    }
}