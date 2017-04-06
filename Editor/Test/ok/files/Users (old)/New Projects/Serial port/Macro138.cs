 Note: this component normally is not licensed

typelib MSCommLib "C:\Documents and Settings\G\Desktop\MSCOMM32.OCX"
VARIANT a=TRUE

MSCommLib.MSComm c._create
c.Settings = "9600,N,8,1"
c.CommPort=1
c.PortOpen = a
out c.PortOpen
c._setevents("c_DMSCommEvents")
opt waitmsg 1
wait -1
