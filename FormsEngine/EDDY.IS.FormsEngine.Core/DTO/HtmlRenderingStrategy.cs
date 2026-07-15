using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.DTO
{
    public class HtmlRenderingStrategy
    {
        public int HtmlRenderingStrategyId { get; set; }
        public string Name { get; set; }
        public string CSSPath { get; set; }
        public string FormTemplateView { get; set; }
        public string ManagedChoiceTemplateView { get; set; }
        public string NoMatchTemplateView { get; set; }
        public string ThankYouTemplateView { get; set; }
        public string NoThankYouTemplateView { get; set; }
        public string CrossSellTemplateView { get; set; }
    }
}
