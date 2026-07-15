using EDDY.IS.FormsEngine.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Caching
{
    public class TemplateMappingCache
    {
        /// <summary>
        /// Key: ProgramId
        /// Value: TemplateId
        /// </summary>
        public Dictionary<int, int> ProgramTemplateDictionary { get; set;}

        /// <summary>
        /// Key: ProgramProductId
        /// Value: TemplateId
        /// </summary>
        public Dictionary<int, int> ProgramProductTemplateDictionary { get; set; }

        /// <summary>
        /// List of templates with Programs bound (System Templates)
        /// </summary>
        public HashSet<int> TemplatesWithProgramsBound { get; set; }


        public TemplateMappingCache() 
        {
            ProgramTemplateDictionary = new Dictionary<int, int>();
            ProgramProductTemplateDictionary = new Dictionary<int, int>();
            TemplatesWithProgramsBound = new HashSet<int>();
        }
    }
}
