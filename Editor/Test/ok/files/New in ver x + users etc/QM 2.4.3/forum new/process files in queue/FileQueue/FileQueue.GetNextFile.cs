function# str&s

 Gets (pops) the oldest added file from the queue.
 Returns previous queue length. Returns 0 if queue is empty; then the caller can break rep loop, or wait a second and try again.


lock FileQueue

int R=m_a.len
if R
	s.Swap(m_a[0])
	m_a.remove(0)
else s.all

lock- FileQueue

ret R
