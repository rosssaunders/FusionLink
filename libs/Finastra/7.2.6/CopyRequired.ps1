$RequiredFiles = @(
    "Sophis.DotNetCore.dll"
    "SophisDotNetToolkit.dll"
    "SophisDotNetToolkitGUI.dll"
    "DevExpress.XtraTreeList.v18.1.dll"
    "SophisDotNetTools.dll"
    "SophisPortfoliosGUI.dll"
    "Sophis.API.Configuration.dll"
    "SophisConfiguration.dll"
    "Sophis.Windows.Widgets.dll"
    "Sophis.Windows.Widgets.Core.dll"
)

foreach ($file in $RequiredFiles) {
    Copy-Item -LiteralPath (Join-Path $env:Sophis_v726_x64 $file) -Destination $PSScriptRoot
}