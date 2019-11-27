using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Helpers
{
    public static class Commoms
    {
        public static bool IsNullOrEmpty(this object value)
        {
            if (value == null)
            {
                return true;
            }
            return string.IsNullOrEmpty(value.ToString());
        }
    }
}
