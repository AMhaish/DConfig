using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DConfigOS_Core.Repositories.Utilities;
using DConfigOS_Core.Models;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Configuration;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using System.Drawing;
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using static DConfigOS_Core.Repositories.IOServices.FoldersAPI;

namespace DConfigOS_Core.Repositories.IOServices
{
    public class AzureBlobsAPI : RepositoryBase<DConfigOS_Core_DBContext>, IFoldersAPI
    {
        private CloudBlobClient BlobClient
        {
            get
            {
                return CloudStorageAccount.Parse(ConfigurationManager.AppSettings["BlobStorageConnectionString"]).CreateCloudBlobClient();
            }
        }

        CloudStorageAccount storageAccount;
        string storageConnection;
        string resourcesContainer;
        string companyResourcesContainer;
        string cdnDomainPath = "https://cdn.dconfig.com";
        string blobContainerDomainPath = "https://dconfig.blob.core.windows.net";
        public AzureBlobsAPI()
        {
            storageConnection = ConfigurationManager.AppSettings["BlobStorageConnectionString"];
            resourcesContainer = ConfigurationManager.AppSettings["ResourcesContainerName"];
            if (SABFramework.Core.SABCoreEngine.Instance.Settings.ContainsKey("CDN"))
                cdnDomainPath = SABFramework.Core.SABCoreEngine.Instance.Settings["CDN"];
            if (SABFramework.Core.SABCoreEngine.Instance.Settings.ContainsKey("BLOB_STORAGE"))
                blobContainerDomainPath = SABFramework.Core.SABCoreEngine.Instance.Settings["BLOB_STORAGE"];
            companyResourcesContainer = SABFramework.PreDefinedModules.MembershipModule.MembershipProvider.Instance.ContextCompanyId.ToString();

            //CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);
            if (!CloudStorageAccount.TryParse(storageConnection, out storageAccount))
            {
                throw new Exception("Add a environment variable named 'BlobStorageConnectionString' with your Azure Blob storage connection string");
            }
        }

        public Task<int> CreateFile(string path, HttpPostedFileBase uploadedFile, out string lastPath, bool? dconfigReq = true)
        {
            lastPath = "";  
            //Removing encoded characters happened cause Htpp encode for query string
            path = Uri.UnescapeDataString(path);
            try
            {
                if (uploadedFile != null && uploadedFile.ContentLength > 0)
                {
                    string fileName = uploadedFile.FileName;

                    // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
                    CloudBlobContainer container = BlobClient.GetContainerReference(resourcesContainer);

                    string directoryPath = "";
                    //It is required to remove the container path
                    if (!path.Contains(companyResourcesContainer))
                    {
                        directoryPath = companyResourcesContainer + '/' +  path.Replace("/" + resourcesContainer + "/", "");
                    } else { 
                        directoryPath = path.Replace("/" + resourcesContainer + "/", "");
                    }

                    var directory = container.GetDirectoryReference(directoryPath);

                    // Get the reference to the block blob from the container
                    CloudBlockBlob blockBlob = directory.GetBlockBlobReference(fileName);
                    if (blockBlob.Exists())
                    {
                        blockBlob = directory.GetBlockBlobReference(fileName.Replace(".", new Random().Next(100000).ToString() + "."));
                    }
                    if (uploadedFile.ContentType != null)
                    {
                        blockBlob.Properties.ContentType = uploadedFile.ContentType;
                    }
                    // Upload the file
                    blockBlob.UploadFromStream(uploadedFile.InputStream);

                    lastPath = blockBlob.Uri.ToString().Replace(blobContainerDomainPath, cdnDomainPath);
                    return Task.FromResult(ResultCodes.Succeed);
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

        public Task<int> CreateWaterMarkFile(string path, HttpPostedFileBase uploadedFile, out string lastPath, string watermarkPath, bool? dconfigReq = true)
        {
            lastPath = "";
            //Removing encoded characters happened cause Htpp encode for query string
            path = Uri.UnescapeDataString(path);
            try
            {
                if (uploadedFile != null && uploadedFile.ContentLength > 0)
                {
                    string fileName = uploadedFile.FileName;
                    fileName = Guid.NewGuid().ToString() + fileName;

                    // ***** Processing Watermark image *****
                    Image sourceImg = Image.FromStream(uploadedFile.InputStream);

                    WebClient client = new WebClient();
                    //We have to add https here cause when passing it as parameter we lose the :
                    Stream stream = client.OpenRead((watermarkPath.StartsWith("http")? watermarkPath: "https://" + watermarkPath));
                    Image waterMarkImg = Image.FromStream(stream);

                    Graphics imageGraphics = Graphics.FromImage(sourceImg);
                    TextureBrush watermarkBrush = new TextureBrush(waterMarkImg);

                    int x = (sourceImg.Width / 2 - waterMarkImg.Width / 2);
                    int y = (sourceImg.Height / 2 - waterMarkImg.Height / 2);
                    watermarkBrush.TranslateTransform(x, y);
                    imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(waterMarkImg.Width + 1, waterMarkImg.Height)));
                    // **************************************

                    // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
                    CloudBlobContainer container = BlobClient.GetContainerReference(resourcesContainer);

                    string directoryPath = "";
                    //It is required to remove the container path
                    if (!path.Contains(companyResourcesContainer))
                    {
                        directoryPath = companyResourcesContainer + '/' + path.Replace("/" + resourcesContainer + "/", "");
                    }
                    else
                    {
                        directoryPath = path.Replace("/" + resourcesContainer + "/", "");
                    }

                    var directory = container.GetDirectoryReference(directoryPath);

                    // Get the reference to the block blob from the container
                    CloudBlockBlob blockBlob = directory.GetBlockBlobReference(fileName);
                    if (blockBlob.Exists())
                    {
                        blockBlob = directory.GetBlockBlobReference(fileName.Replace(".", new Random().Next(100000).ToString() + "."));
                    }

                    if (uploadedFile.ContentType != null)
                    {
                        blockBlob.Properties.ContentType = uploadedFile.ContentType;
                    }
                    // Upload the file

                    var imgStream = new System.IO.MemoryStream();
                    sourceImg.Save(imgStream, ImageFormat.Png);
                    imgStream.Position = 0;
                    blockBlob.UploadFromStream(imgStream);

                    lastPath = blockBlob.Uri.ToString().Replace(blobContainerDomainPath, cdnDomainPath);
                    return Task.FromResult(ResultCodes.Succeed);
                }
                else
                {
                    return Task.FromResult(ResultCodes.ObjectEmpty);
                }
            }
            catch (Exception ex)
            {
                SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Error in uploading a file to cloud",ex);
                return Task.FromResult(ResultCodes.UnknownError);
            }
        }

        //Create Container
        public Task<int> CreateFolder(string path)
        {
            try
            {
                var uniEncodeByte = UnicodeEncoding.Unicode.GetBytes(path);
                var uniEncodeStr = UnicodeEncoding.Unicode.GetString(uniEncodeByte);
                //Removing encoded characters happened cause Htpp encode for query string
                path = Uri.UnescapeDataString(uniEncodeStr);

                // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
                CloudBlobContainer container = BlobClient.GetContainerReference(resourcesContainer);
                //It is required to remove the container path
                path = path.Replace("//", "/");
                var directoryPath = path.Replace("/" + resourcesContainer + "/", "");
                var directory = container.GetDirectoryReference(directoryPath);
                //Adding temp file so we can see the blob directory
                CloudBlockBlob blockBlob = directory.GetBlockBlobReference(".settings");
                string temp = "Settings";
                var memoryStream = new System.IO.MemoryStream(Encoding.ASCII.GetBytes(temp));
                blockBlob.UploadFromStream(memoryStream);
                //Should create here
                return Task.FromResult(ResultCodes.Succeed);
            }
            catch
            {
                return Task.FromResult(ResultCodes.UnknownError);
            }
        }

        public Task<int> DeleteFile(string path)
        {
            try
            {
                // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
                CloudBlobContainer container = BlobClient.GetContainerReference(resourcesContainer);
                var blobPath = path.Replace(cdnDomainPath + "/" + resourcesContainer + "/", "");
                //blobPath = blobPath.Replace(cdnDomainPath, blobContainerDomainPath);
                var blockBlob = container.GetBlockBlobReference(blobPath);
                blockBlob.Delete();

                //Should create here
                return Task.FromResult(ResultCodes.Succeed);
            }
            catch (Exception ex)
            {
                return Task.FromResult(ResultCodes.UnknownError);
            }
        }

        public Task<int> DeleteFolder(string path)
        {
            try
            {
                // Get reference to the blob container by passing the name by reading the value from the configuration (appsettings.json)
                CloudBlobContainer container = BlobClient.GetContainerReference(resourcesContainer);
                var blobPath = path.Replace("/" + resourcesContainer + "/", "");

                var directoryBlob = container.GetDirectoryReference(blobPath);

                foreach (IListBlobItem item in directoryBlob.ListBlobs())
                {
                    if (item.GetType() == typeof(CloudBlob) || item.GetType().BaseType == typeof(CloudBlob))
                    {
                        ((CloudBlob)item).DeleteIfExists();
                    }
                }

                //Should create here
                return Task.FromResult(ResultCodes.Succeed);
            }
            catch
            {
                return Task.FromResult(ResultCodes.UnknownError);
            }
        }

        public async Task<List<IFolder>> GetChildFolders(string path)
        {
            //Need to apply the right path before fetching
            List<IFolder> finalList = new List<IFolder>();
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var rootContainer = cloudBlobClient.GetContainerReference(resourcesContainer);
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                var blobPath = path.Replace("/" + resourcesContainer + "/", "");
                var results = await rootContainer.ListBlobsSegmentedAsync(blobPath, blobContinuationToken);
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    if (item.GetType() == typeof(CloudBlobDirectory))
                    {
                        CloudBlobDirectory directory = (CloudBlobDirectory)item;
                        //Removing last / from directory
                        string name = directory.Prefix.Remove(directory.Prefix.Length - 1);
                        var con = new BlobDirectory()
                        {
                            Name = name.Split('/').Last(),
                            Path = directory.Uri.AbsolutePath
                        };
                        finalList.Add(con);

                    }
                }
            } while (blobContinuationToken != null);
            return finalList;
        }

        public async Task<List<IFile>> GetFolderFiles(string path)
        {
            //Need to apply the right path before fetching
            List<IFile> finalList = new List<IFile>();
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var rootContainer = cloudBlobClient.GetContainerReference(resourcesContainer);
            BlobContinuationToken blobContinuationToken = null;
            do
            {
                path = path.Replace("//", "/");
                var blobPath = path.Replace("/" + resourcesContainer + "/", "");
                var results = await rootContainer.ListBlobsSegmentedAsync(blobPath, blobContinuationToken);
                blobContinuationToken = results.ContinuationToken;
                foreach (IListBlobItem item in results.Results)
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {                        
                        CloudBlob blob = (CloudBlob)item;
                        string newPath = blob.Uri.ToString().Replace(blobContainerDomainPath, cdnDomainPath);
                        var bo = new Blob()
                        {
                            Name = blob.Name.Split('/').Last(),
                            Path = newPath,
                            Length = blob.Properties.Length,
                            Type = blob.Properties.ContentType,
                            CDNPath = newPath
                        };
                        var fileName = bo.Name.Replace(blobPath, "");
                        if (fileName != ".settings")
                            finalList.Add((IFile)bo);
                    }
                }
            } while (blobContinuationToken != null);
            return finalList;
        }

        public async Task<IFolder> GetResourcesFolder(string rootPath = null, string userRootPath = null)
        {
            CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
            // Codee for creating the root folder if not exists
            var root = await GetChildFolders('/' + resourcesContainer + '/');
            var companyRoot = root.Where(x => x.Name == companyResourcesContainer).FirstOrDefault();
            if(root.Count <= 0 || companyRoot == null)
            {
                await CreateFolder('/' + resourcesContainer + '/' + companyResourcesContainer);
                await CreateFolder('/' + resourcesContainer + '/' + companyResourcesContainer + "/Resources");
            }
            var items = await GetChildFolders('/' + resourcesContainer + '/' + companyResourcesContainer + '/');
            //var rootContainer = cloudBlobClient.GetContainerReference(resourcesContainer);
            //BlobContinuationToken blobContinuationToken = null;
            //do
            //{
            //var results = await rootContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
            //blobContinuationToken = results.ContinuationToken;
            foreach (IFolder item in items)
            {
                //if (item.GetType() == typeof(CloudBlobDirectory))
                if (item.GetType() == typeof(BlobDirectory))
                {
                    //CloudBlobDirectory directory = (CloudBlobDirectory)item;
                    BlobDirectory directory = (BlobDirectory)item;
                    //if (directory.Prefix == "Resources/")
                    if (directory.Path.Contains("Resources/"))
                    {
                        var con = new BlobDirectory()
                        {
                            //Name = directory.Prefix.Substring(0, directory.Prefix.Length - 1),
                            Name = directory.Name,
                            //Path = directory.Uri.AbsolutePath
                            Path = directory.Path
                        };
                        return new Folder(con.Name, con.Path);
                    }
                }
            }
            //} while (blobContinuationToken != null);
            return null;
        }

        public Task<int> MoveFile(string oldPath, string newPath)
        {
            throw new NotImplementedException();
        }

        public async Task<int> MoveFolder(string destContainerName, string srcContainerName)
        {
            try
            {
                CloudBlobClient BlobClient = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("ResourcesContainerName")).CreateCloudBlobClient();
                var srcContainer = BlobClient.GetContainerReference(srcContainerName);
                var destContainer = BlobClient.GetContainerReference(destContainerName);

                List<string> sourceUris = new List<string>();
                BlobContinuationToken blobContinuationToken = null;
                do
                {
                    var results = await srcContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    blobContinuationToken = results.ContinuationToken;
                    foreach (IListBlobItem item in results.Results)
                    {
                        if (item.GetType() == typeof(CloudBlob))
                        {
                            CloudBlob blob = (CloudBlob)item;
                            sourceUris.Add(item.Uri.AbsoluteUri);
                        }
                    }
                } while (blobContinuationToken != null);

                foreach (var uri in sourceUris)
                {
                    string filename = System.IO.Path.GetFileName(uri);
                    CloudBlockBlob sourceBlockBlob = srcContainer.GetBlockBlobReference(filename);
                    CloudBlockBlob targetBlockBlob = destContainer.GetBlockBlobReference(filename);

                    await targetBlockBlob.StartCopyAsync(sourceBlockBlob);
                    sourceBlockBlob.Delete();
                }

                return ResultCodes.Succeed;
            }
            catch (Exception ex)
            {
                return ResultCodes.UnknownError;
            }
        }
    }
}
