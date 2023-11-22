using CompetitiveAnalysis.Models;
using DConfigOS_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitiveAnalysis.ProductsManagerServices
{
    public interface IPropertiesAPI
    {
        List<Property> GetProperties();
        List<Property> GetGroupProperties(int groupId);
        List<FieldsType> GetPropertiesTypes();
        PropertiesGroup GetPropertiesGroup(int groupId);
        List<PropertiesGroup> GetPropertiesGroups();
        Property GetProperty(int propertyId);
        int CreateProperty(Property p);
        int CreatePropertiesGroup(PropertiesGroup g);
        int UpdateProperty(Property p);
        int UpdatePropertiesGroup(PropertiesGroup p);
        int UpdateGroupsOrder(List<PropertiesGroup> groups);
        int DeleteProperty(int Id);
        int DeletePropertiesGroup(int Id);
        int AddPropertyToGroup(int groupId, Property property);
        int UpdateGroupProperty(Property property);
        int RemovePropertyFromGroup(int propertId);

    }
}
