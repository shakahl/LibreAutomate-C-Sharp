 /Macro1663
function $commandBar commandId


WS

Q &q

typelib Office {2DF8D04C-5BFA-101B-BDE5-00AA0044DE52} 2.3
Office.CommandBars c=ws.Application.CommandBars
Office.CommandBar b=c.Item(commandBar)
Office.CommandBarControl x=b.FindControl(@ commandId @ 0 1)
 out x
Q &qq
outq

x.Execute

 if a.a
	 a.DoDefaultAction


 err+ end _error
