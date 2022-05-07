using System;

namespace ContactSample.Models
{
    public class ContactFormModel
    {
        public string IP { get; set; } = "not-set";
        public string Name{ get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Comments { get; set; }
        public string LogSerialized 
        {
            get
            {
                return $"IP: { IP } | Name: { Name } | Email: { Email } | Phone: { Phone } | Comments: { Comments }";

            }
        }
    }
}
