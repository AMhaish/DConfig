using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DConfigOS_Core.Models;
using DConfigOS_Core.Models.Utilities;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using DConfigOS_Core.Repositories.Utilities;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public class StylesBundlesAPI : IStylesBundlesAPI
    {
        public virtual List<StylesBundle> GetStylesBundles(string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var styles = context.StylesBundles.ToList();
            return styles;
        }

        public virtual StylesBundle GetStylesBundle(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            return context.StylesBundles.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).SingleOrDefault();
        }

        public virtual int CreateStylesBundle(StylesBundle styleSheet, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStyle = context.StylesBundles.Where(m => m.Name == styleSheet.Name && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbStyle == null)
            {
                styleSheet.CreateDate = DateTime.Now;
                context.StylesBundles.Add(styleSheet);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectAlreadyExists;
            }
        }

        public virtual int UpdateStylesBundle(StylesBundle styleSheet, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStyle = context.StylesBundles.Where(m => m.Id == styleSheet.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            var sameStyle = context.StylesBundles.Where(m => m.Name == styleSheet.Name && m.Id != styleSheet.Id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbStyle != null)
            {
                if (sameStyle == null)
                {
                    dbStyle.Name = styleSheet.Name;
                    context.SaveChanges();
                    return ResultCodes.Succeed;
                }
                else
                {
                    return ResultCodes.ObjectNameAlreadyUsed;
                }
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

        public virtual int DeleteStylesBundle(int id, string creatorId = null)
        {
            DConfigOS_Core_DBContext context = new DConfigOS_Core_DBContext();
            var dbStyle = context.StylesBundles.Where(m => m.Id == id && (creatorId == null || m.CreatorId == creatorId)).FirstOrDefault();
            if (dbStyle != null)
            {
                var styles = dbStyle.Styles.ToList();
                foreach (var s in styles)
                {
                    context.StyleSheets.Remove(s);
                }
                context.StylesBundles.Remove(dbStyle);
                context.SaveChanges();
                return ResultCodes.Succeed;
            }
            else
            {
                return ResultCodes.ObjectHasntFound;
            }
        }

      
    }
}
