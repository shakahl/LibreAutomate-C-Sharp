SYSTEM_INFO k
GetSystemInfo &k
out "0x%X 0x%X 0x%X 0x%X" k.dwPageSize k.dwAllocationGranularity k.lpMinimumApplicationAddress k.lpMaximumApplicationAddress
