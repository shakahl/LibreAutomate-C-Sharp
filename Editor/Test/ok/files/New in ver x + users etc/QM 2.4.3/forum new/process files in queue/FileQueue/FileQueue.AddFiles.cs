function# $files_

 Adds (pushes) one or more files to the queue.
 Returns previous queue length.


lock FileQueue

int R=m_a.len
foreach _s files_
	m_a[]=_s

lock- FileQueue

ret R
