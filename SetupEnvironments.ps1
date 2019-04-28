$VersionsSupported = @(
    "7.1.3.14"
    "7.1.3.19"
    "7.2.6"
)

foreach($version in $VersionsSupported) {

    $path = Read-Host -Prompt "Enter the path to the $version installation root folder (Leave blank if N/A)"

    if(![string]::IsNullOrWhitespace($path)) {

        if(Test-Path $path) {

            $versionWithoutDots = $version.Replace(".","") 
            $versionCode = "Sophis_v" + $versionWithoutDots + "_x64"
        
            [Environment]::SetEnvironmentVariable($versionCode, $path, "User")
        }
    }
}
