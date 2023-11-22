using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IStylesBundlesAPI
    {
        List<StylesBundle> GetStylesBundles(string creatorId = null);
        StylesBundle GetStylesBundle(int id, string creatorId = null);
        int CreateStylesBundle(StylesBundle styleSheet, string creatorId = null);
        int UpdateStylesBundle(StylesBundle styleSheet, string creatorId = null);
        int DeleteStylesBundle(int id, string creatorId = null);

    }
}
