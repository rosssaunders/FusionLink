using System;
using sophis.configuration;
using sophis.market_data;
using Sophis.API.Configuration;

namespace sophis
{
    internal class CSMApiHost : CSMApi
    {
        private static string _userName;

        // static holder for instance, need to use lambda to construct since constructor private
        private static readonly Lazy<CSMApiHost> _instance = new Lazy<CSMApiHost>(() => Initialize(_userName));

        public static bool IsApiInitialized
        {
            get
            {
                return _instance.IsValueCreated;
            }
        }

        public static void SetUsername(string userName)
        {
            _userName = userName;
        }

        private static CSMApiHost Initialize(string userName)
        {
            var api = new CSMApiHost(userName);

            return api;
        }

        // private to prevent direct instantiation.
        private CSMApiHost(string userName)
        {
            _userName = userName;
        }

        public void InitializeReportApi()
        {
            //Initializing the .net Framework causes the XML bootstrapper to populate the ProgramConfiguration settings. 
            initFrameWork();

            //With .net initialized, we can load the Configuration
            ProgramConfiguration.Current.LoadConfiguration();

            if ((ProgramConfiguration.Current == null) || (ProgramConfiguration.Current.Configuration == null))
            {
                throw new ApplicationException("Unable to set the LastUserName.");
            }

            SetUser(_userName);

            Initialise();
        }

        // accessor for instance
        public static CSMApiHost Instance => _instance.Value;

        /// <summary>
        /// Sets the Sophis User to load the API as.
        /// </summary>
        /// <remarks>
        /// This is a bit of black magic to get Sophis running under the correct user name.
        /// Basically, since the CSMApi doesn't parse the commandline for the  -I parameter, it will load the previously loaded user from the Config file.
        /// So what we do is force the Config file to load prior to Sophis initializing, and set the LastUserName.
        /// </remarks>
        /// <param name="userName">The Sophis UserName to set</param>
        private void SetUser(string userName)
        {
            //If everything goes well, we can set the LastUserName of the APIConfiguration section
            var common = (APIConfigurationGroup)ProgramConfiguration.Current.Configuration.SectionGroups.Get("APIConfiguration");
            common.API.LastUserName = userName;
        }

        private void SetDate(DateTime systemDate)
        {
            var common = (APIConfigurationGroup)ProgramConfiguration.Current.Configuration.SectionGroups.Get("APIConfiguration");

            if (common.Parameters == null)
                throw new ApplicationException("Please add the Parameters section to the Risk.config file.");
        }
    }
}
