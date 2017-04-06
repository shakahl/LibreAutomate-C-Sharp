CsFunc("" 50) ;;here 50 is brightness, 0 to 100


#ret
//C# code
using System;
using System.Management;

public class Test
{
public static void SetBrightness(byte targetBrightness)
{
ManagementScope scope = new ManagementScope("root\\WMI");
SelectQuery query = new SelectQuery("WmiMonitorBrightnessMethods");
using(ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
{
using(ManagementObjectCollection objectCollection = searcher.Get())
{
foreach(ManagementObject mObj in objectCollection)
{
mObj.InvokeMethod("WmiSetBrightness", new Object[] { UInt32.MaxValue, targetBrightness });
break;
}
}
}
}
}
