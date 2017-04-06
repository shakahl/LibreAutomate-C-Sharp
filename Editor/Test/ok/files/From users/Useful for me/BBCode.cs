str Type Name Trigger BBC ForumC TEMPx TEMPz
Type.getmacro("" 3)
if val(Type)=0;Type="[b][color=green]Macro[/color][/b]"
if val(Type)=1;Type="[b][color=cyan]Function[/color][/b]"
if val(Type)=2;Type="[b][color=violet]Menu[/color][/b]"
if val(Type)=3;Type="[b][color=darkblue]Toolbar[/color][/b]"
if val(Type)=4;Type="[b][color=white]T. S. Menu[/color][/b]"
if val(Type)=6;Type="[b][color=green]Member Function[/color][/b]"
Name.getmacro("" 1)
Trigger.getmacro("" 2)
if (!len(Trigger)=0);TEMPz.formata("[b][color=red]Trigger[/color][/b] ( %s )" Trigger)
act id(2210 _hwndqm)
'Ca
men 33126 _hwndqm
0.1
'R
BBC.getclip
0.1
TEMPx=
 %s ( %s ) %s
 %s
ForumC.formata(TEMPx Type Name TEMPz BBC)
ForumC.setclip
