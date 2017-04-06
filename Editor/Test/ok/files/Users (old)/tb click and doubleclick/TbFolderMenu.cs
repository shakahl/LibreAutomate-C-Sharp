 /
function $path [$path2]

int+ g_tbclicktime
int t=GetTickCount
if(t-g_tbclicktime>500)
	str s sp(path)
	if(len(path2)) sp.formata("'' ''%s" path2)
	s.format(" /expandfolders[]. ''%s''" sp)
	DynamicMenu s
else run path
g_tbclicktime=t
