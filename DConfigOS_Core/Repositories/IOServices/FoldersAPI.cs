using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using DConfigOS_Core.Models.Utilities;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using DConfigOS_Core.Repositories.Utilities;
using System.Web;
using System.Net;

namespace DConfigOS_Core.Repositories.IOServices
{
    public class FoldersAPI : RepositoryBase<DConfigOS_Core_DBContext>, IFoldersAPI
    {
        public class Folder : IFolder
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public string CDNPath { get; set; }

            public Folder(DirectoryInfo d) : this(d.Name, d.FullName) { }

            public Folder(string name, string fullPath)
            {
                Name = name;
                Path = fullPath.Replace(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath, "").Replace("\\", "/");
                if (SABFramework.Core.SABCoreEngine.Instance.Settings.ContainsKey(SABFramework.Core.SABSettings.SABSettings_CDN))
                {
                    CDNPath = fullPath.Replace(SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath, SABFramework.Core.SABCoreEngine.Instance.Settings[SABFramework.Core.SABSettings.SABSettings_CDN]).Replace("\\", "/");
                }
                else
                {
                    CDNPath = Path;
                }

            }

            public static string GetFullPath(string path)
            {
                return SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + path.Replace("/", "\\");
            }
        }

        public class File : Folder, IFile
        {
            public string Type { get; set; }
            public long Length { get; set; }
            public DateTime CreationDate { get; set; }
            public DateTime LastModifiedDate { get; set; }

            public File(FileInfo f) : base(f.Name, f.FullName)
            {
                Type = f.Extension;
                Length = f.Length;
                CreationDate = f.CreationTime;
                LastModifiedDate = f.LastWriteTime;
            }
        }

        public virtual Task<List<IFolder>> GetChildFolders(string path)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(Folder.GetFullPath(path));
                List<IFolder> result = dir.GetDirectories().Select(m => new Folder(m)).ToList<IFolder>();
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in reading directories", ex);
                return null;
            }
        }

        public virtual Task<List<IFile>> GetFolderFiles(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(Folder.GetFullPath(path));
            var files = dir.GetFiles().Select(m => new File(m)).ToList<IFile>();
            return Task.FromResult(files);
        }

        public virtual Task<IFolder> GetResourcesFolder(string rootPath = null, string userRootPath = null)
        {
            try
            {
                DirectoryInfo dir;
                //if (SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.CurrentUserIsAdministrator)
                //{
                //    var directoryPath = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "/GlobalResources" + (String.IsNullOrEmpty(rootPath) ? "" : rootPath);
                //    dir = new DirectoryInfo(directoryPath);
                //}
                //else
                //{
                var directoryPath = SABFramework.Core.SABCoreEngine.Instance.AppPhysicalPath + "/GlobalResources/" + SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.ContextCompanyId + "/Resources" + (String.IsNullOrEmpty(rootPath) ? (String.IsNullOrEmpty(userRootPath) ? "" : userRootPath) : rootPath);
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                dir = new DirectoryInfo(directoryPath);
                //}
                return Task.FromResult<IFolder>(new Folder(dir));
            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in reading directories", ex);
                return null;
            }
        }

        public virtual Task<int> CreateFolder(string path)
        {
            try
            {
                DirectoryInfo dir = Directory.CreateDirectory(Folder.GetFullPath(path));
                return Task.FromResult(ResultCodes.Succeed);
            }
            catch
            {
                return Task.FromResult(ResultCodes.UnknownError);
            }
        }

        public virtual Task<int> CreateFile(string path, HttpPostedFileBase uploadedFile, out string lastPath, bool? dconfigReq = true)
        {
            lastPath = null;
            try
            {
                if (uploadedFile != null && uploadedFile.ContentLength > 0)
                {
                    bool savedWithAnotherName = false;
                    string fileName = uploadedFile.FileName;
                    if (dconfigReq.HasValue && !dconfigReq.Value)
                        path = "/GlobalResources/" + SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.ContextCompanyId + "/Resources" + path;
                    string savingPath = Path.Combine(Folder.GetFullPath(path), fileName);
                    if (!Directory.Exists(Folder.GetFullPath(path)))
                    {
                        Directory.CreateDirectory(Folder.GetFullPath(path));
                    }
                    while (System.IO.File.Exists(savingPath))
                    {
                        fileName = fileName.Replace(".", new Random().Next(100000).ToString() + ".");
                        savingPath = Path.Combine(Folder.GetFullPath(path), fileName);
                        savedWithAnotherName = true;
                    }
                    uploadedFile.SaveAs(savingPath);
                    lastPath = path + "/" + fileName;
                    if (savedWithAnotherName)
                    {
                        return Task.FromResult(ResultCodes.ObjectSavedWithAnotherName);
                    }
                    else
                    {
                        return Task.FromResult(ResultCodes.Succeed);
                    }
                }
                else
                {
                    return Task.FromResult(ResultCodes.ObjectEmpty);
                }
            }
            catch(Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error when writing the file on the cloud", ex);
                return Task.FromResult(ResultCodes.UnknownError);
            }
        }

        public virtual Task<int> CreateWaterMarkFile(string path, HttpPostedFileBase uploadedFile, out string lastPath, string watermarkPath, bool? dconfigReq = true)
        {
            lastPath = null;
            try
            {
                if (uploadedFile != null && uploadedFile.ContentLength > 0)
                {
                    bool savedWithAnotherName = false;
                    string fileName = uploadedFile.FileName;
                    if (dconfigReq.HasValue && !dconfigReq.Value)
                        path = "/GlobalResources/" + SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.ContextCompanyId + "/Resources" + path;
                    string savingPath = Path.Combine(Folder.GetFullPath(path), fileName);
                    if (!Directory.Exists(Folder.GetFullPath(path)))
                    {
                        Directory.CreateDirectory(Folder.GetFullPath(path));
                    }
                    while (System.IO.File.Exists(savingPath))
                    {
                        fileName = fileName.Replace(".", new Random().Next(100000).ToString() + ".");
                        savingPath = Path.Combine(Folder.GetFullPath(path), fileName);
                        savedWithAnotherName = true;
                    }

                    // ***** Processing Watermark image *****
                    Image sourceImg = Image.FromStream(uploadedFile.InputStream);

                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead(watermarkPath);
                    Image waterMarkImg = Image.FromStream(stream);

                    Graphics imageGraphics = Graphics.FromImage(sourceImg);
                    TextureBrush watermarkBrush = new TextureBrush(waterMarkImg);

                    int x = (sourceImg.Width / 2 - waterMarkImg.Width / 2);
                    int y = (sourceImg.Height / 2 - waterMarkImg.Height / 2);
                    watermarkBrush.TranslateTransform(x, y);
                    imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(waterMarkImg.Width + 1, waterMarkImg.Height)));
                    // **************************************

                    sourceImg.Save(savingPath);

                    lastPath = path + "/" + fileName;
                    if (savedWithAnotherName)
                    {
                        return Task.FromResult(ResultCodes.ObjectSavedWithAnotherName);
                    }
                    else
                    {
                        return Task.FromResult(ResultCodes.Succeed);
                    }
                }
                else
                {
                    return Task.FromResult(ResultCodes.ObjectEmpty);
                }
            }
            catch
            {
                return Task.FromResult(ResultCodes.UnknownError);
            }
        }


        public virtual Task<int> MoveFolder(string oldPath, string newPath)
        {
            try
            {
                Directory.Move(Folder.GetFullPath(oldPath), Folder.GetFullPath(newPath));
                return Task.FromResult(ResultCodes.Succeed);
            }
            catch (DirectoryNotFoundException ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Couldn't find directory", ex);
                return Task.FromResult(ResultCodes.ObjectHasntFound);
            }
            catch
            {
                return Task.FromResult(ResultCodes.UnknownError);
            }
        }

        public virtual Task<int> MoveFile(string oldPath, string newPath)
        {
            try
            {
                System.IO.File.Move(File.GetFullPath(oldPath), File.GetFullPath(newPath));
                return Task.FromResult(ResultCodes.Succeed);
            }
            catch (DirectoryNotFoundException ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Couldn't find directory", ex);
                return Task.FromResult(ResultCodes.ObjectHasntFound);
            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Uknown error", ex);
                return Task.FromResult(ResultCodes.UnknownError);
            }
        }

        public virtual Task<int> DeleteFolder(string path)
        {
            try
            {
                Directory.Delete(Folder.GetFullPath(path), true);
                return Task.FromResult(ResultCodes.Succeed);
            }
            catch (DirectoryNotFoundException ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Couldn't find directory", ex);
                return Task.FromResult(ResultCodes.ObjectHasntFound);
            }
            catch
            {
                return Task.FromResult(ResultCodes.UnknownError);
            }
        }

        public virtual Task<int> DeleteFile(string path)
        {
            try
            {
                System.IO.File.Delete(File.GetFullPath(path));
                return Task.FromResult(ResultCodes.Succeed);
            }
            catch (DirectoryNotFoundException ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Couldn't find directory", ex);
                return Task.FromResult(ResultCodes.ObjectHasntFound);
            }
            catch
            {
                return Task.FromResult(ResultCodes.UnknownError);
            }
        }
    }
}
