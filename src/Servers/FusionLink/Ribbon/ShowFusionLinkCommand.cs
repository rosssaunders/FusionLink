//  Copyright (c) RXD Solutions. All rights reserved.


using System.Windows.Input;
using RxdSolutions.FusionLink.Properties;
using Sophis.Windows;
using Sophis.Windows.Ribbon;

namespace RxdSolutions.FusionLink.Ribbon
{
    public class ShowFusionLinkCommand : RibbonCommand
    {
        protected ShowFusionLinkCommand() : base(Resources.CommandShowFusionLinkScenarioText, 
                                                 Resources.CommandShowFusionLinkScenarioName, 
                                                 typeof(ShowFusionLinkCommand), 
                                                 "monitor_rgb")
        {
        }

        public static ShowFusionLinkCommand Register()
        {
            var cmd = new ShowFusionLinkCommand();

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
            Main.CaptionBar.Show();
        }
    }
}
