 /scheduler
function

type SCHEDULEDTASK
	str'name
	DateTime'tStart
	repeatEvery
	everyWhat ;;1 min, 2 hour, 3 day, 4 week, 5 month
	weekDays ;;flags: 1 mon, 2 tue, 4 wed, 8 thu, 16 fri, 32 sat, 64 sun
	action ;;1 run, 2 mac
	str'path str'args
	flags
ARRAY(SCHEDULEDTASK)- a
SCHEDULEDTASK& t
int i nc; lpstr k

 first time or when schedule changed, parse schedule CSV and store in ARRAY(SCHEDULEDTASK) a
if g_scheduleUpdate
	lock schedule
	g_scheduleUpdate=0
	ICsv c._create; c.FromString(g_schedule)
	lock- schedule
	nc=c.ColumnCount
	a.create(c.RowCount)
	for i 0 a.len
		&t=a[i]
		t.name=c.Cell(i 0)
		t.tStart.FromStr(c.Cell(i 1)) ;;info: error if incorrect format
		k=c.Cell(i 2); t.repeatEvery=val(k 0 _i); if(t.repeatEvery) t.everyWhat=SelStr(0 k+_i "m" "h" "d" "w" "M"); if(!t.everyWhat) end F"invalid interval: {k}"
		k=c.Cell(i 3); t.action=SelStr(0 k "run" "mac"); if(!t.action) end F"invalid action: {k}"
		t.path=c.Cell(i 4)
		if(nc>5) t.args=c.Cell(i 5)
		if(nc>6) t.flags=val(c.Cell(i 6))
		 out _s.getstruct(t 1) ;;debug
	out "Schedule updated" ;;debug

 wait 1 s
wait 1

 get current time
DateTime tNow.FromComputerTime
int mNow; int-- mPrev; tNow.GetParts(0 0 0 0 mNow); if(mNow=mPrev) ret; else mPrev=mNow ;;check with precision of every 1 minute
out tNow.ToStr(4) ;;debug
int M d h m wd _M _d _h _m _wd
tNow.GetParts(0 M d h m 0 0 0 wd)

 for each scheduled task
for i 0 a.len
	&t=a[i]
	 compare time
	if(tNow<t.tStart or t.flags&1) continue
	t.tStart.GetParts(0 _M _d _h _m 0 0 0 _wd)
	int runNow=(_m=m and _h=h and _d=d and _M=M)
	 reschedule if need
	sel t.everyWhat
		case 0 t.flags|1
		case 1 rep() t.tStart.AddParts(0 0 t.repeatEvery); if(t.tStart>tNow) break ;;not the best way, but easy
		case 2 rep() t.tStart.AddParts(0 t.repeatEvery); if(t.tStart>tNow) break
		case 3 rep() t.tStart.AddParts(t.repeatEvery); if(t.tStart>tNow) break
		case 4 rep() t.tStart.AddParts(t.repeatEvery*7); if(t.tStart>tNow) break
		case 5 rep() t.tStart.AddMonths(t.repeatEvery); if(t.tStart>tNow) break
	 is it late to run task now?
	if !runNow
		sel t.action
			case 1 out F"<>late: {t.name} at {t.tStart.ToStr}. <link ''{t.path} /{t.args}''>Run now</link>."
			case 2 out F"<>late: {t.name} at {t.tStart.ToStr}. <macro ''{t.path} /{t.args}''>Run now</macro>."
		continue
	 run task
	sel t.action
		case 1 run t.path t.args; err out F"failed: run ''{t.path}'' ''{t.args}''"
		case 2 mac t.path t.args; err out F"failed: mac ''{t.path}'' ''{t.args}''"
