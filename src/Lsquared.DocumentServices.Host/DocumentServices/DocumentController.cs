using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ViewFeatures;

namespace Lsquared.DocumentServices.Host
{
    public sealed class DocumentController
    {
        [ViewDataDictionary]
        public ViewDataDictionary ViewData { get; set; }

        public DocumentController(IEnumerable<IDocumentSharingService> services)
        {
            _services = services;
        }

        public IActionResult List(string path = "/", string format = "HTML")
        {
            var items = new List<object>(50);
            foreach (var service in _services)
            {
                items.AddRange(service.ListFolders(path));
                items.AddRange(service.ListDocuments(path));
            }

            if (string.Equals(format, "JSON", StringComparison.OrdinalIgnoreCase))
            {
                return new JsonResult(items);
            }
            else if (string.Equals(format, "HTML", StringComparison.OrdinalIgnoreCase))
            {
                ViewData.Model = items;
                return new ViewResult { ViewData = ViewData, ViewName = "list" };
            }

            return new BadRequestResult();
        }

        private readonly IEnumerable<IDocumentSharingService> _services;
    }
}
