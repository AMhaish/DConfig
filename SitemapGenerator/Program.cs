using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Providers.ResourcesProviders;
using Ninject;
using System.Net.Mail;

namespace SitemapGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = (args.Length <= 0 ? System.Configuration.ConfigurationManager.AppSettings["path"] : args[0]);
            SitemapProvider provider = new SitemapProvider(new DConfigOS_Core.Repositories.WebsiteContentServices.ContentsAPI());
            provider.InitializeSitemap(path, (ex) =>
            {
                MailMessage smtpmessage = new MailMessage();
                smtpmessage.Body = "Exception in generating sitemaps" + "<br/>";
                if (ex != null)
                {
                    smtpmessage.Body += ex.Message + "<br/>";
                    smtpmessage.Body += ex.StackTrace;
                }
                smtpmessage.To.Add(System.Configuration.ConfigurationManager.AppSettings["supportEmail"]);
                smtpmessage.Subject = "Exception in generating sitemaps";
                SmtpClient client = new SmtpClient();
                client.Send(smtpmessage);
            });
        }
    }
}
