using EDDY.IS.FormsEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.FormsEngine.Services.Models
{
    public class TemplateControlModel
    {
        public List<TemplateControlDTO> ControlList { get; set; }
        public bool InlineDropDownRender { get; set; }
    }
}