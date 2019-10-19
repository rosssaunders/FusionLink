//  Copyright (c) RXD Solutions. All rights reserved.


using System.Windows.Input;
using RxdSolutions.FusionLink.Properties;
using Sophis.Windows;
using Sophis.Windows.Ribbon;

namespace RxdSolutions.FusionLink.Ribbon
{
    public class StartStopDataServerCommand : RibbonCommand
    {
        protected StartStopDataServerCommand() : base(Resources.CommandStartStopButtonText, 
                                                      Resources.CommandStartStopButtonName, 
                                                      typeof(StartStopDataServerCommand), 
                                                      "trafficlight_on")
        {
        }

        public static StartStopDataServerCommand Register()
        {
            var cmd = new StartStopDataServerCommand();

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
            if (Main.DataServer.IsRunning)
                Main.DataServer.Stop();
            else
                Main.DataServer.Start();
        }
    }
}
