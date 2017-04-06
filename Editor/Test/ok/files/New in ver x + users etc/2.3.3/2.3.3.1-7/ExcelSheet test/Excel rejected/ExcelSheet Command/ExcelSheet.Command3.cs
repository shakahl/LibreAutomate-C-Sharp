 /Macro1663
function $commandPath
 function $commandPath $item

 Tried to execute menu and tb commands, but it is not so easy.
 Easier with acc (see Command2), but slower.
 Don't need this, because:
 If Excel visible, can use key, and will use because easier.
 If Excel hidden, can do almost everything with ExcelSheet and Excel functions.

WS

Q &q
ARRAY(str) a=commandPath
 if(a.len<2) end ERR_BADARG

typelib Office {2DF8D04C-5BFA-101B-BDE5-00AA0044DE52} 2.3
Office.CommandBars c=ws.Application.CommandBars
Office.CommandBar b=c.Item(a[0])
 out b
 Office.CommandBarControl x=b.Controls.Item(1)
 out x.Caption
  b.Controls.Item(1).Execute
 x.Execute

Q &qq

Office.CommandBarControl x



 foreach x b.Controls
	 out x.Caption
 x=b.Controls.Item("Options...")
 out x
 out x.Tag
 x=b.FindControl(@ @ item 0 1)
 out x

 int i
 for i 1 a.len
Q &qqq
 outq

 if a.a
	 a.DoDefaultAction


 err+ end _error
