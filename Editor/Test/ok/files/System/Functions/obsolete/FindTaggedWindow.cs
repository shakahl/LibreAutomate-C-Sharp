 /
function# $tag $name $classname [$exename] [flags]

 Finds window tagged by TagWindow.

 See also: <TagWindow>.


int i
for i 1 1000000
	int h=win(name classname exename flags 0 0 i)
	if(!h) ret
	if(GetProp(h tag)) ret h
