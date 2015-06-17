using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace Lsquared.DocumentServices.Host
{
    public class DocumentController
    {
        public DocumentController(IEnumerable<IDocumentSharingService> services)
        {
            _services = services;
        }

        public IActionResult List(string path = "/", string format = null)
        {
            throw new NotImplementedException();
        }

        private readonly IEnumerable<IDocumentSharingService> _services;
    }
}
