echo ***************************************************************************
echo Pokit 💩
echo ***************************************************************************
echo "Generte the README.html."
echo "Starting..."
echo "This current path is " """$PSScriptRoot"""

try
{
    echo "This current pip version:"
    pip "-V"
}
catch
{
    Write-Host "Caught an exception:" -ForegroundColor Red
    Write-Host "Exception Type: $($_.Exception.GetType().FullName)" -ForegroundColor Red
    Write-Host "Exception Message: $($_.Exception.Message)" -ForegroundColor Red
    echo "Please install python & pip and add the python to path."
}
try
{
    echo "This current grip version:"
    grip "-V"
}
catch
{
    Write-Host "Caught an exception:" -ForegroundColor Red
    Write-Host "Exception Type: $($_.Exception.GetType().FullName)" -ForegroundColor Red
    Write-Host "Exception Message: $($_.Exception.Message)" -ForegroundColor Red
    echo "Please install grip by pip."
}

echo "Exporting..."
cd $PSScriptRoot

grip README.md --export README.html
# grip README.md --export README.html -q 2>&1 | %{ "$_" }  #"-q 2>&1 | %{ "$_" }" 为了清除ps中的错误报错信息（调用第三方应用执行时有时即使执行成功ps还是会报错）
echo "Success."
echo ***************************************************************************
echo Pokit 💩
echo ***************************************************************************