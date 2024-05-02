using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsAppIntegration_CPC
{
    public class OptinDetails
    {

        public string method { get; set; }
        public string userid { get; set; }
        public string password { get; set; }
        public string phone_number { get; set; }
        public string v { get; set; }
        public string auth_scheme { get; set; }
        public string channel { get; set; } 
    }
    public class MessageJsonDetails
    {
        public string userid { get; set; }
        public string password { get; set; }
        public string send_to { get; set; }
        public string v { get; set; } 
        public string format { get; set; }
        public string msg_type { get; set; }
        public string method { get; set; }
        public string msg { get; set; }
        //public string isTemplate { get; set; }
        //public string header { get; set; }


    }
}
