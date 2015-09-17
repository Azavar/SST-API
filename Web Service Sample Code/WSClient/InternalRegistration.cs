using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSClient
{
    /// <summary>
    /// Sample registration structure a State may use for internal use.
    /// Used for demonstrating the tranformation from the SST Schema to another arbitrary representation.
    /// </summary>
    public class InternalRegistration
    {
        public string SSTID;
        public string TIN;
        public string TINType;
        public string First;
        public string Middle;
        public string Last;
        public string BusinessNameLine1;
        public string BusinessNameLine2;
        public string DBA;

        public string MailingAddressLine1;
        public string MailingAddressLine2;
        public string MailingCity;
        public string MailingStateProvince;
        public string MailingPostalCode;
        public string MailingCountry;

        public string DocumentId;


    }
}
