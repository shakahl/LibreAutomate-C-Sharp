 \pipe_window
function ms

rep
	int mSize mCount
	if(!GetMailslotInfo(ms 0 &mSize &mCount 0) or !mCount) break
	 out "%i %i" mCount mSize MAILSLOT_NO_MESSAGE
	if mSize>0
		_s.all(mSize-1 2)
		if(!ReadFile(ms _s mSize &_i 0)) break
		 out _i
		out _s
		 out _s.left(_s 1)
	else ReadFile(ms 0 0 &_i 0) ;;get empty message
