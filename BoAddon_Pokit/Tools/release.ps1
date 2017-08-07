<#
**** Programmer:Harold.Duan
**** Date:20170613
**** Reason:Release addon's program and generate the addon's register data file.
#>

param(
[string]$APP_NAME=$(throw "Parameter missing: -app_name APP_NAME"),
[string]$VERSION=$(throw "Parameter missing: -version VERSION"),
[string]$PLATFORM=$(throw "Parameter missing: -platform PLATFORM"),
[string]$OUTPUT_PATH=$(throw "Parameter missing: -output_path OUTPUT_PATH")
)


"APP_NAME= $APP_NAME"
"PLATFORM=$PLATFORM"
"OUTPUT_PATH=$OUTPUT_PATH"

$file_path = Split-Path -Parent $MyInvocation.MyCommand.Definition
$xml_file_name = "B1AddOnRegData"
$APP_NAME = $APP_NAME.Replace(".exe","")

if($PLATFORM -ieq "x64")
{
    $xml_file_name = $file_path + "\" + $xml_file_name + ".x64.xml"
}
else
{
    $xml_file_name = $file_path + "\" + $xml_file_name + ".x86.xml"
}

echo $xml_file_name

[xml]$xml_file_data = [xml](Get-Content -Path $xml_file_name)
$xml_file_data.AddOnInfo.addonname = $APP_NAME

echo $xml_file_data.AddOnInfo.addonname

$temp_file_name = $OUTPUT_PATH = $OUTPUT_PATH + "temp.xml"
Out-File -FilePath $temp_file_name -Encoding utf8 -Force -InputObject $xml_file_data.InnerXml

echo $xml_file_name
echo $temp_file_name
echo $VERSION
echo $OUTPUT_PATH
echo "$OUTPUT_PATH\$APP_NAME.exe"

#cmd /c "$file_path\AddOnRegDataGen.exe" $temp_file_name $VERSION "$OUTPUT_PATH\$APP_NAME"
