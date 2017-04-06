str vbs=
 function ChassisType
 strComputer = "."
 Set objWMIService = GetObject("winmgmts:" _
 & "{impersonationLevel=impersonate}!\\" _
 & strComputer & "\root\cimv2")
 Set colChassis = objWMIService.ExecQuery _
 ("Select * from Win32_SystemEnclosure")
 For Each objChassis in colChassis
 For Each objItem in objChassis.ChassisTypes
 ChassisType = objItem
 exit function
 Next
 Next
 end function
VbsAddCode vbs
out VbsFunc("ChassisType")
