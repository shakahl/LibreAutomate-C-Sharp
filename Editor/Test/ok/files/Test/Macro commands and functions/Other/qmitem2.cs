ClearOutput
QMITEM q
out qmitem("qmitem2" 0 q 255)
out q.datemod
out q.filter
out q.flags
out q.folderid
out q.htvi
out q.itype
out q.name
out q.programs
out q.tht
out q.tkey
out q.tkey2
out q.tmod
out q.tmon
out q.trigger
out q.triggerdescr
out q.ttype
out q.text

 int i=0
 rep
	 i=qmitem(-i 0 q 1)
	 if(!i) break
	 out q.name
	