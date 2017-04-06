
 Closes all running instances of exe before compiling it.
 Use this function in 'Run before' field of Make Exe dialog, or call from a function that is used in 'Run before'.

 Added in: QM 2.3.3.


ARRAY(int) a; int i
win "" "QM_ExeManager" _command 0 0 0 a
for i 0 a.len
	clo a[i]; err
	wait 10 WP a[i]
	err
ret 1
