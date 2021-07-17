using SquareDMS.DataLibrary.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquareDMS.ActionFilters
{
    public class BaseActionFilter
    {
        /// <summary>
        /// Trims all properties of the dto if the property is a string and not null
        /// </summary>
        public static void TrimAllProperties(IDataTransferObject dto)
        {
            // checks each property if its a string and trims the value
            foreach (var property in dto.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(dto);

                if (propertyValue != null && propertyValue is string)
                {
                    var propertyValueString = propertyValue as string;

                    property.SetValue(dto, propertyValueString.Trim());
                }
            }
        }
    }
}
