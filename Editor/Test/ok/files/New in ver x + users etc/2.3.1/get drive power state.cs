 __HFile h.Create("\\.\PhysicalDrive2" OPEN_EXISTING GENERIC_READ FILE_SHARE_READ|FILE_SHARE_WRITE)
__HFile h.Create("\\.\PhysicalDrive2" OPEN_EXISTING GENERIC_READ FILE_SHARE_READ|FILE_SHARE_WRITE)
 __HFile h.Create("\\.\e:\" OPEN_EXISTING GENERIC_READ FILE_SHARE_READ|FILE_SHARE_WRITE)
out h
out GetDevicePowerState(h &_i)
out _i
