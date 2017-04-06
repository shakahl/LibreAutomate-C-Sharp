str regcode="..."
mes- "This will add your registration code to the registry and restart Quick Macros.[][]%s" "" "OCi" regcode
if(!rset(regcode "QMX" "Software\gindi")) mes- "Failed." "" "x"
shutdown -2



 Run this macro using the Run button on the toolbar.
 It shows a message box. Click OK.

 If it does not show a message box, double click
 your computer clock and temporarily change computer
 time to the time when QM still worked.
