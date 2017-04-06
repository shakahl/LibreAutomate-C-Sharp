 /
function# $driveLetter [&partitionNumber]

 Gets disk and partition number of drive.
 Returns disk number.
 Error if fails.


__Handle hf=CreateFileW(@F"\\.\{driveLetter}:" 0 0 0 OPEN_EXISTING 0 0); if(hf=-1) end "failed, %s" 0 _s.dllerror

STORAGE_DEVICE_NUMBER d
if(!DeviceIoControl(hf IOCTL_STORAGE_GET_DEVICE_NUMBER 0 0 &d sizeof(d) &_i 0)) end "failed, %s" 0 _s.dllerror

if(&partitionNumber) partitionNumber=d.PartitionNumber
ret d.DeviceNumber
