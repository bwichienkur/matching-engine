using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces
{
    public interface IComponentRenderingService
    {
        List<string> RenderComponents(string templateKey, IEnumerable<object> models);
    }
}
