using System;
using System.Collections.Generic;

namespace Lsquared.DocumentServices
{
    public class SharingDataServiceFacade
    {
    }

    public class FolderRepository
    {
        public IEnumerable<Folder> GetFolders(string path)
        {
            throw new NotImplementedException();
        }
    }

    public class DocumentRepository
    {
        public IEnumerable<Document> GetDocuments(string path)
        {
            throw new NotImplementedException();
        }
    }
}
