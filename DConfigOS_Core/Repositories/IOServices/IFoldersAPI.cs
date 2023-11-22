using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static DConfigOS_Core.Repositories.IOServices.FoldersAPI;

namespace DConfigOS_Core.Repositories.IOServices
{
    public interface IFoldersAPI : IDisposable
    {
        Task<List<IFolder>> GetChildFolders(string path);
        Task<List<IFile>> GetFolderFiles(string path);
        Task<IFolder> GetResourcesFolder(string rootPath = null, string userRootPath = null);
        Task<int> CreateFolder(string path);     
        Task<int> CreateFile(string path, HttpPostedFileBase uploadedFile, out string lastPath, bool? dconfigReq = true);
        Task<int> CreateWaterMarkFile(string path, HttpPostedFileBase uploadedFile, out string lastPath, string watermarkPath, bool? dconfigReq = true);
        Task<int> MoveFolder(string oldPath, string newPath);
        Task<int> MoveFile(string oldPath, string newPath);
        Task<int> DeleteFolder(string path);
        Task<int> DeleteFile(string path);
    }
    public interface IFolder
    {
        string Name { get; set; }
        string Path { get; set; }
        string CDNPath { get; set; }
    }
    public interface IFile : IFolder
    {
        string Type { get; set; }
        long Length { get; set; }
        DateTime CreationDate { get; set; }
        DateTime LastModifiedDate { get; set; }
    }
}
