 Performance counter functions.
 Can be used, for example, to create performance triggers.
 The counters are the same as you can see in Performance Monitor (perfmon.exe).
 At first call Open. Then repeatedly call Query. The first Query does not get results.

 EXAMPLES
out
#compile __CPerformanceCounter
CPerformanceCounter c
 c.Open("\LogicalDisk(C:)\% Disk Time") ;;logical disk C:
 c.Open("\PhysicalDisk(_Total)\% Disk Time") ;;all physical disks (the same as GetDiskUsage)
 c.Open("\Memory\Available MBytes") ;;free physical memory
c.Open("\Network Interface(*)\Bytes Received/sec") ;;bytes received/s by all network adapters.
c.Query
rep 5
	1
	out c.Query
