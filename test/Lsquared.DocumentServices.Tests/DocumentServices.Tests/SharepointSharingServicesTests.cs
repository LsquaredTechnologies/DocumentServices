using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Lsquared.DocumentServices.Tests.DocumentServices.Tests
{
    public abstract class SharepointSharingServicesTests
    {
        [Fact]
        public void Test1()
        {
            var sharepoint = new SharepointSharingService(GetOptions());

            var folders = sharepoint.ListFolders("/Documents partages/Documents publics/");

            var files = sharepoint.ListDocuments("/Documents partages/Documents publics/");

            var document = sharepoint.ShareDocument("/Documents partages/Documents publics/", "test.pdf", File.OpenRead("demo.pdf"));
        }

        protected virtual SharepointSharingOptions GetOptions()
        {
            return new SharepointSharingOptions
            {
                UserName = "***@***.com",
                Password = "p4s$W0Rd",
                Tenant = "***" // Part before '.sharepoint.com'
            };
        }
    }
}
