$RequiredFiles = @(
    "SophisDotNetToolkit.dll"
    "DevExpress.XtraTreeList.v11.2.dll"
    "Sophis.Windows.Widgets.dll"
    "SophisDotNetToolkit.dll"
    "SophisDotNetToolkitGUI.dll"
    "SophisDotNetTools.dll"
    "SophisPortfoliosGUI.dll"
)

foreach ($file in $RequiredFiles) {
    Copy-Item -LiteralPath (Join-Path $env:Sophis_v71314_x64 $file) -Destination $PSScriptRoot
}