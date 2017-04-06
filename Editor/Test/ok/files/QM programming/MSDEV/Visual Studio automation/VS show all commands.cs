 Displays all available Visual Studio commands.
 They can be used like
   CVisualStudio x
   x.dte.ExecuteCommand("command" ...)


#compile "__CVisualStudio"
CVisualStudio x

out

ARRAY(str) a
EnvDTE.Command c
foreach c x.dte.Commands
	str s=c.Name
	if(s.len) a[]=s

a.sort

int i
for i 0 a.len
	out a[i]
