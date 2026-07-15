using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.Util.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class CustomDTOMappingAttribute : Attribute
    {
        private string _Mapping { get; set; }

        public CustomDTOMappingAttribute(string Mapping)
        {
            _Mapping = Mapping;
        }

        public override string ToString()
        {
            return _Mapping;
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class CustomDataModelMappingAttribute : Attribute
    {
        private string _Mapping { get; set; }

        public CustomDataModelMappingAttribute(string Mapping)
        {
            _Mapping = Mapping;
        }

        public override string ToString()
        {
            return _Mapping;
        }
    }
}
