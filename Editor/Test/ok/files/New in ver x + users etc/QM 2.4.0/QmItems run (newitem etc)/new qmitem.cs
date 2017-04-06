out
dll "qm.exe" TestQmitem [id] [flags] [QMITEM*pi] [mask]
1
QMITEM q
PF
rep 3
	 _i=TestQmitem("test3")
	_i=TestQmitem("test3" 0 &q -1)
	 _i=TestQmitem("test.xml" 0 &q -1)
	 _i=TestQmitem(3 0 &q -1)
	PN
PO
out _i
out _s.getstruct(q 1)
out q.datemod

PF
int i n
rep
	i=TestQmitem(-i 0 &q 1)
	if(!i) break; else n=i
	out q.name
PN;PO
out n

#ret

 int qmitem([$name|iid] [flags] [QMITEM&qi] [mask])   ;;`Finds QM item, and gets properties.`
 flags: 1 skipfolders, 2 skipshared, 4 skipencrypted, 8 skipdisabled, 16 skipwithouttrigger,
    32 skipmemberf, 64 skiplink.
 mask: 1 name, 2 trigger, 4 programs, 8 text, 16 folderid, 32 filter, 64 descr, 128 datemod,
    0x100 file link target.
