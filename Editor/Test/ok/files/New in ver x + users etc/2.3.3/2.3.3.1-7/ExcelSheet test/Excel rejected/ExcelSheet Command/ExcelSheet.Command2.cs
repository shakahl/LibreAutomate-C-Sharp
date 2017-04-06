 /Macro1663
function $commandPath


WS

 typelib Office {2DF8D04C-5BFA-101B-BDE5-00AA0044DE52} 2.3
 Office.CommandBars c=ws.Application.CommandBars
 Office.CommandBar b=c.Item(1)
  Office.CommandBarButton x=+b.Controls.Item(1)
 Office.CommandBarControl x=b.Controls.Item(1)
 out x.Caption
  b.Controls.Item(1).Execute
 x.Execute

Q &q
ARRAY(str) ar=commandPath
if(ar.len<2) end ERR_BADARG
int cb=child(ar[0] "MsoCommandBar" ExcelHwnd)
outw cb
 Acc a=acc(cb)
Acc a.FromWindow(cb OBJID_CLIENT)
int i
Q &qq
for i 1 ar.len
	a.Find(a.a "" ar[i] "" 1|16|64)
	out a.a
	if(!a.a) break
Q &qqq
outq

if a.a
	a.DoDefaultAction


 err+ end _error
