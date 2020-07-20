//  Copyright (c) RXD Solutions. All rights reserved.


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RxdSolutions.FusionLink.ExcelClient
{
    public class ConnectionBuilder
    {
        private readonly Uri _uri;

        public ConnectionBuilder(Uri uri)
        {
            _uri = uri;
        }

        public int GetProcessId()
        {
            return Convert.ToInt32(_uri.Segments[3].Replace("/", ""));
        }

        public int GetSessionId()
        {
            return Convert.ToInt32(_uri.Segments[2].Replace("/", ""));
        }

        public string GetMachineName()
        {
            var machineName = _uri.Host.Substring(0, _uri.Host.IndexOf('.'));

            return machineName.ToUpper();
        }

        public string GetConnectionUsername()
        {
            return _uri.Segments[4].Replace("/", "");
        }

        public string GetConnectionName()
        {
            try
            {
                return $"{GetMachineName()}:{GetProcessId()}";
            }
            catch
            {
                return _uri.ToString();
            }
        }
    }
}
