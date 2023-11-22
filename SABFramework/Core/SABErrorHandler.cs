using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;
using Microsoft.AspNet.Identity;
using SABFramework.Providers;
using System.Threading.Tasks;

namespace SABFramework.Core
{
    public class SABErrorHandler
    {
        private string AppPhysicalPath;
        private IEmailProvider emailProvider;
        private static readonly object _syncObject = new object();

        public SABErrorHandler(IEmailProvider emailProvider) { this.emailProvider = emailProvider; }

        public SABErrorHandler() { }

        public SABErrorHandler(string appPhysicalPath) { AppPhysicalPath = appPhysicalPath; }

        public SABErrorHandler(IEmailProvider emailProvider, string appPhysicalPath)
        {
            this.emailProvider = emailProvider;
            AppPhysicalPath = appPhysicalPath;
        }

        public virtual void ProcessError(string message, Exception ex = null)
        {
            LogTxtErrors(message, ex);
            LogCsvErrors(message, ex);
        }

        protected virtual void LogTxtErrors(string message, Exception ex = null)
        {
            lock (_syncObject)
            {
                FileStream logStream = null;
                StreamWriter writer = null;
                try
                {
                    if (!System.IO.Directory.Exists(AppPhysicalPath + "\\logs"))
                    {
                        System.IO.Directory.CreateDirectory(AppPhysicalPath + "\\logs");
                    }
                    logStream = new FileStream(AppPhysicalPath + "\\logs\\" + DateTime.Now.ToString("dd-MM-yyyy") + ".txt", FileMode.Append, FileAccess.Write, FileShare.None, 4096, true);
                    writer = new StreamWriter(logStream);
                    writer.WriteLine(DateTime.Now.ToShortTimeString() + ":");
                    if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
                    {
                        writer.WriteLine(System.Web.HttpContext.Current.Server.HtmlEncode(System.Web.HttpContext.Current.Request.Url.AbsoluteUri));
                    }
                    if (!String.IsNullOrEmpty(message))
                    {
                        writer.WriteLine(message);
                        writer.WriteLine();
                    }
                    if (ex != null)
                    {
                        if (ex.Message != null)
                            writer.WriteLine(ex.Message);
                        var exObj = ex;
                        do
                        {
                            exObj = exObj.InnerException;
                            if (exObj != null && exObj.Message != null)
                            {
                                writer.WriteLine(exObj.Message);
                            }

                    } while (exObj != null);
                    if (ex.StackTrace != null)
                        writer.WriteLine(ex.StackTrace);
                }
            }
            finally
            {
                if (writer != null && logStream != null)
                {
                    writer.WriteLine();
                    writer.Flush();
                    logStream.Close();
                }

                }
            }
        }

        protected virtual void LogCsvErrors(string message, Exception ex = null)
        {
            lock (_syncObject)
            {
                FileStream logStream = null;
                StreamWriter writer = null;
                try
                {
                    if (!System.IO.Directory.Exists(AppPhysicalPath + "\\logs"))
                    {
                        System.IO.Directory.CreateDirectory(AppPhysicalPath + "\\logs");
                    }
                    logStream = new FileStream(AppPhysicalPath + "\\logs\\" + DateTime.Now.ToString("dd-MM-yyyy") + ".csv", FileMode.Append, FileAccess.Write, FileShare.None, 4096, true);
                    writer = new StreamWriter(logStream);
                    writer.Write(DateTime.Now.ToShortDateString() + "#");
                    writer.Write(DateTime.Now.ToShortTimeString() + "#");
                    if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
                    {
                        writer.Write(System.Web.HttpContext.Current.Server.HtmlEncode(System.Web.HttpContext.Current.Request.Url.Host) + "#");
                    }
                    else
                    {
                        writer.Write("None" + "#");
                    }
                    if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Request != null)
                    {
                        writer.Write(System.Web.HttpContext.Current.Server.HtmlEncode(System.Web.HttpContext.Current.Request.Url.AbsoluteUri) + "#");
                    }
                    else
                    {
                        writer.Write("None" + "#");
                    }
                    if (ex != null)
                    {
                        writer.Write(ex.GetType() + "#");
                    }
                    else
                    {
                        writer.Write("No exception" + "#");
                    }
                    if (!String.IsNullOrEmpty(message))
                    {
                        writer.Write(message + "#");
                    }
                    else
                    {
                        writer.Write("None" + "#");
                    }
                    //if (ex != null)
                    //{
                    //    if (ex.Message != null)
                    //        writer.Write(ex.Message + " - ");
                    //    var exObj = ex;
                    //    do
                    //    {
                    //        exObj = exObj.InnerException;
                    //        if (exObj != null && exObj.Message != null)
                    //        {
                    //            writer.Write(exObj.Message.Replace('\n','-') + " - ");
                    //        }

                    //    } while (exObj != null);
                    //    if (ex.StackTrace != null)
                    //        writer.Write(ex.StackTrace.Replace('\n', '-') + " - ");
                    //    writer.Write("#");
                    //}
                    //else
                    //{
                    //    writer.Write("None" + "#");
                    //}
                }
                finally
                {
                    writer.WriteLine();
                    writer.Flush();
                    logStream.Close();
                }
            }
        }

        public virtual void SendEmailNotification(string title, string message, Exception ex = null)
        {
            if (emailProvider != null)
            {
                try
                {
                    IdentityMessage smtpmessage = new IdentityMessage();
                    smtpmessage.Body = message + "<br/>";
                    if (ex != null)
                    {
                        smtpmessage.Body += ex.Message + "<br/>";
                        smtpmessage.Body += ex.StackTrace;
                    }
                    smtpmessage.Destination = SABCoreEngine.Instance.Settings["ErrorsEmail"];
                    smtpmessage.Subject = title;
                    Task.Run(() => emailProvider.SendAsync(smtpmessage)).Wait();
                }
                catch (Exception newex)
                {
                    SABFramework.Core.SABCoreEngine.Instance.ErrorHandler.ProcessError("Email send process failed", newex);
                }
            }
            else
            {
                throw new Exception("Email provider dependency is not passed to the error handler");
            }
        }
    }
}
