 /
function $name

lock schedule
ICsv c._create; c.FromFile(g_scheduleFile)
int i=c.Find(name 1); if(i<0) ret
int flags=val(c.Cell(i 6)); if(flags&1) ret
c.Cell(i 6)=F"{flags|1}"
c.ToFile(g_scheduleFile)
