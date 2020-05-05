using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace model
{
    class UserLog
    {

        private string user;
        private string postazione;
        private string priorita;
        private DateTime logTimestamp;

        public UserLog(string user, string postazione, string priorita, DateTime logTimestamp)
        {
            this.user = user;
            this.postazione = postazione;
            this.priorita = priorita;
            this.logTimestamp = logTimestamp;
        }

        override
        public String ToString() {
            return "Log di priorità "+priorita+" generato da "+user+" dalla postazione "+postazione+" alle "+logTimestamp;
        }

        public string User { get => user; set => user = value; }
        public string Postazione { get => postazione; set => postazione = value; }
        public string Priorita { get => priorita; set => priorita = value; }
        public DateTime LogTimestamp { get => logTimestamp; set => logTimestamp = value; }

    }

}