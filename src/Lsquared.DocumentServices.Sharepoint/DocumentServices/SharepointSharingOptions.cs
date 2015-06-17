using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lsquared.DocumentServices
{
    public class SharepointSharingOptions
    {
        public string UserName
        {
            get; set;
        }

        public string Password
        {
            get; set;
        }

        public string Tenant
        {
            get; set;
        }

        public string Domain
        {
            get
            {
                return Tenant + ".sharepoint.com";
            }
        }
    }
}
