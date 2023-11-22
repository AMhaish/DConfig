using CompetitiveAnalysis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompetitiveAnalysis.ProductsManagerServices
{
    public interface IPropertiesEnumsAPI
    {
        List<PropertyEnum> GetPropertiesEnums();
        PropertyEnum GetPropertiesEnum(int id);
        int CreatePropertyEnum(PropertyEnum propertyEnum);
        int UpdatePropertyEnum(PropertyEnum propertyEnum);
        int DeletePropertyEnum(int id);
        int UpdatePropertyEnumValues(int enumId, List<PropertyEnumValue> values);
        int DeletePropertyEnumValue(int id);

    }
}
