using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServices
{
    public class RestClientConfigItems
    {
        public string baseUri { get; set; }
        public string clientId { get; set; }
        public string service { get; set; }

        public bool oidcEnabled = true;
    }
}
