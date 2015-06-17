using System.Collections.Generic;
using System.IO;

namespace Lsquared.DocumentServices
{
    public interface IDocumentSharingService
    {
        IEnumerable<Folder> ListFolders(string folderPath);

        IEnumerable<Document> ListDocuments(string folderPath);

        Document ShareDocument(string remoteFolderPath, string fileName, Stream file);
    }
}
