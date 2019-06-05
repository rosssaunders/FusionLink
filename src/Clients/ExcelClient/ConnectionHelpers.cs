using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public static string GetConnectionName(Uri uri)
        {
            try
            {
                using (var process = Process.GetProcessById(GetConnectionId(uri)))
                {
                    var title = process.MainWindowTitle;
                    if (string.IsNullOrWhiteSpace(title))
                        title = $"{process.ProcessName} - {process.Id}";

                    return $"{process.Id} - {title}";
                }
            }
            catch
            {
                return uri.ToString();
            }
        }
    }
}
