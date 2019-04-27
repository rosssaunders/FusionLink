using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public static class ConnectionHelper
    {
        public static int GetConnectionId(Uri uri)
        {
            return Convert.ToInt32(uri.ToString().Split('_')[1]);
        }
    }
}
