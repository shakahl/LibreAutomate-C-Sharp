function [event] [pid] [pidparent] [$name]
 event: 1 started, 2 ended, 4 running

lock ;;prevent using global variables simultaneously by multiple threads

type PIDNAME pid ~name
ARRAY(PIDNAME)+ g_proc
int i found

sel event
	case 0 ;;called from init2 at QM startup. Add pid/name of all processes to g_proc.
	ARRAY(int) ap; ARRAY(str) as
	EnumProcessesEx &ap &as
	g_proc.create(ap.len)
	for(i 0 ap.len) g_proc[i].pid=ap[i]; g_proc[i].name.Swap(as[i])
	
	case 1 ;;process started. Add pid/name to g_proc.
	PIDNAME& r=g_proc[]
	r.pid=pid; r.name=name
	
	if(event=1) out F"process started: pid={pid}, name={name}"
	
	case 2 ;;process ended. Find pid/name in g_proc and remove from g_proc.
	for(i 0 g_proc.len) if(g_proc[i].pid=pid) found=1; break
	if(!found) out F"pid {pid} not found"; ret ;;should never happen
	
	out F"process ended: pid={pid}, name={g_proc[i].name}"
	
	g_proc.remove(i)
