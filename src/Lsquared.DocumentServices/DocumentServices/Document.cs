using System;

namespace Lsquared.DocumentServices
{
    public class Document
    {
        public string Name
        {
            get; set;
        }

        public string Path
        {
            get; set;
        }

        public Uri Uri
        {
            get; set;
        }

        public Uri ViewUri
        {
            get; set;
        }
    }
}
