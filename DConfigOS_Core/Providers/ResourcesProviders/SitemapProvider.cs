using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using DConfigOS_Core.Repositories.WebsiteContentServices;
using SABFramework.Core;
using System.IO;
using Ninject;

namespace DConfigOS_Core.Providers.ResourcesProviders
{
    public class SitemapProvider
    {
        [Inject]
        private IContentsAPI contentsAPI;

        public SitemapProvider(IContentsAPI contentsAPI) { this.contentsAPI = contentsAPI; }

        private static SitemapProvider _instance;
        public static SitemapProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SitemapProvider(
                        SABFramework.Core.SABCoreEngine.Instance.DInjector.Get<IContentsAPI>()
                        );
                }
                return _instance;
            }
        }

        public void InitializeSitemap(string path = null, Action<Exception> exceptionHandler=null, bool contextFree=true)
        {
            string finalPath = (path ?? SABCoreEngine.Instance.AppPhysicalPath);
            new Thread(() =>
            {
                try
                {
                    var domains = contentsAPI.GetDomains(null, contextFree);
                    foreach (var d in domains)
                    {
                        if (!String.IsNullOrEmpty(d.DomainAliases))
                        {
                            string hostName = d.DomainAliases.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)[0];
                            var contents = contentsAPI.GetPagesContents(d.Id);
                            //Check the directory existence
                            if (!Directory.Exists(finalPath + "\\Sitemaps"))
                            {
                                Directory.CreateDirectory(finalPath + "\\Sitemaps");
                            }
                            string indexPath = finalPath + "\\Sitemaps\\" + d.ContextCompanyId.ToString();
                            if (File.Exists(indexPath) && File.GetLastAccessTime(indexPath) > DateTime.Today.Subtract(new TimeSpan(7, 0, 0, 0)))
                            {
                                continue;
                            }
                            StringBuilder SB = new StringBuilder();
                            SB.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                            SB.AppendLine("<urlset xmlns=\"http://www.google.com/schemas/sitemap/0.9\">");
                            foreach (Content c in contents)
                            {
                                if (c.ContentType == ContentTypes.Page && !String.IsNullOrEmpty(c.Name) && !String.IsNullOrEmpty(c.UrlName) && c.ContentInstances != null)
                                {
                                    foreach (ContentInstance ci in c.ContentInstances)
                                    {
                                        SB.AppendLine("<url>");
                                        SB.AppendLine("<loc>http://" + hostName + (!String.IsNullOrEmpty(ci.Language) ? "/" + ci.Language : "") + c.UrlFullCode + "</loc>");
                                        SB.AppendLine("<lastmod>" + DateTime.Now.Year.ToString() + "-" + (DateTime.Now.Month < 10 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + "-" + (DateTime.Now.Day < 10 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString()) + "</lastmod>");
                                        SB.AppendLine("<changefreq>monthly</changefreq>");
                                        SB.AppendLine("<priority>0.9</priority>");
                                        SB.AppendLine("</url>");
                                    }
                                }
                            }
                            SB.AppendLine("</urlset>");
                            FileStream streamIndex = new FileStream(indexPath, FileMode.Create, FileAccess.ReadWrite);
                            StreamWriter IndexFile = new StreamWriter(streamIndex);
                            IndexFile.Write(SB.ToString());
                            IndexFile.Flush();
                            IndexFile.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    exceptionHandler?.Invoke(ex);
                }
            }).Start();
        }

    }
}
