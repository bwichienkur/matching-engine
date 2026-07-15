using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace EDDY.IS.FormsEngine.DTO
{
    public class PixelCheckDTO
    {
        public bool ServiceCalled { get; set; }
        public string Pixels { get; set; }
        public string PixelsWithDebugInfo { get; set; }

        public PixelCheckDTO()
        {
            ServiceCalled = false;
        }
    }
}