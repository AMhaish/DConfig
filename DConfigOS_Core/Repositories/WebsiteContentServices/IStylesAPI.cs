using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DConfigOS_Core.Repositories.WebsiteContentServices
{
    public interface IStylesAPI
    {
        List<StyleSheet> GetStyles();
        StyleSheet GetStyle(int id);
        int CreateStyle(StyleSheet styleSheet);
        int UpdateStyle(StyleSheet styleSheet);
        int UpdateStylesOrder(List<StyleSheet> styles, string creatorId = null);
        int DeleteStyle(int id);

    }
}
