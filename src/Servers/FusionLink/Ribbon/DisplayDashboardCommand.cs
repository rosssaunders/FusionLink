using System;
using System.Windows.Input;
using RxdSolutions.FusionLink.Client;
using RxdSolutions.FusionLink.Properties;
using Sophis.Windows;
using Sophis.Windows.Integration;
using Sophis.Windows.Ribbon;

namespace RxdSolutions.FusionLink.Ribbon
{
    public class DisplayDashboardCommand : RibbonCommand
    {
        protected DisplayDashboardCommand() : base(Resources.CommandDisplayDashboardText, 
                                                   Resources.CommandDisplayDashboardName, 
                                                   typeof(DisplayDashboardCommand),
                                                   "radar-chart")
        {   
        }

        public static DisplayDashboardCommand Register()
        {
            var cmd = new DisplayDashboardCommand();

            RibbonCommands.RegisterDynamic(cmd);
            SophisApplication.MainCommandTarget.CommandBindings.Add(new CommandBinding(cmd, new ExecutedRoutedEventHandler(Handler), new CanExecuteRoutedEventHandler(CanExecuteHandler)));

            return cmd;
        }

        private static void CanExecuteHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private static void Handler(object sender, ExecutedRoutedEventArgs e)
        {
            var wndKey = new WindowKey(3049, 0, 0);
            IntPtr activeWindow = WPFAdapter.Instance.GetActiveWindow(wndKey);
            if (IntPtr.Zero != activeWindow)
            {
                WPFAdapter.Instance.ActivateWindow(activeWindow);
            }
            else
            {
                var fwkElement = new DiagnosticsView(new DiagnosticsViewModel(Main.DataServer));
                WPFAdapter.Instance.OpenWindow(fwkElement, Resources.DashboardWindowCaption, wndKey, false);
            }
        }
    }
}
