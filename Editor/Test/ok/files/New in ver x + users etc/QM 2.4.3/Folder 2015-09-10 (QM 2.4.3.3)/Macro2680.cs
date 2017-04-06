str s=
 <?xml version="1.0" encoding="ISO-8859-8"?>
 <Task version="1.1" xmlns="http://schemas.microsoft.com/windows/2004/02/mit/task">
   <RegistrationInfo>
     <Author>QM</Author>
   </RegistrationInfo>
   <Triggers>
     <TimeTrigger>
       <StartBoundary>2014-07-07T07:16:30</StartBoundary>
     </TimeTrigger>
   </Triggers>
   <Principals>
     <Principal id="Author">
       <LogonType>InteractiveToken</LogonType>
       <RunLevel>LeastPrivilege</RunLevel>
     </Principal>
   </Principals>
   <Actions Context="Author">
     <Exec>
       <Command>notepad.exe</Command>
     </Exec>
   </Actions>
 </Task>
IXml x._create
x.FromString(s)
