using System.Runtime.CompilerServices;
using System.Diagnostics;
namespace EDDY.IS.Vendor.Utilities
{
    public static class Log
    {

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethodName()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return string.Format("{0}.{1}", sf.GetMethod().DeclaringType, sf.GetMethod().Name);
        }

       
    }
}
