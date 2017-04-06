function# action ^waitMax &getHwnd ;;action: 0 created, 1 destroyed, 2 active, 3 visible, 4 hidden, 5 enabled, 6 disabled

if(!m_a.len) end ES_INIT
if(waitMax<0 or waitMax>2000000) end ES_BADARG
opt waitmsg -1
opt hidden -1
if(m_period<=0) m_period=100

int wt(waitMax*1000) t1(GetTickCount) i w
rep
	for i 0 m_a.len
		WWMITEM& r=m_a[i]
		int b(r.exeOrOwner.vt=VT_BSTR) f(r.flags)
		if(b) str s=r.exeOrOwner; else int h=r.exeOrOwner
		if(m_func) f|0x8000
		
		sel r.func
			case 0
				if(b) w=win(r.txt r.cls s f m_func m_param r.matchindex)
				else w=win(r.txt r.cls h f m_func m_param r.matchindex)
			case 1
				if r.id
					if(b) w=child(r.id r.txt r.cls s f m_func m_param r.matchindex)
					else w=child(r.id r.txt r.cls h f m_func m_param r.matchindex)
				else
					if(b) w=child(r.txt r.cls s f m_func m_param r.matchindex)
					else w=child(r.txt r.cls h f m_func m_param r.matchindex)
			case 2
				if(b) w=id(r.id s f)
				else w=id(r.id h f)
			case 3
				if(b) w=win(r.txt "#32770" s f|0x8000 &WaitWinMulti_MsgCallback &r r.matchindex)
				else w=win(r.txt "#32770" h f|0x8000 &WaitWinMulti_MsgCallback &r r.matchindex)
		
		err+
			if(_error.code!=510) end _error ;;not ERR_WINDOW
			sel action
				case [1,4] w=0
				case else end _error
		
		if(w)
			sel action
				case 1 continue
				case 2
					sel r.func
						case [0,3] if(w!=win) continue
						case else if(w!=child) continue
						if(IsIconic(w)) continue
						if hid(w)
							err
							continue
				case [3,4]
					_i=hid(w); err _i=1
					if(action=3) if(_i) continue
					else if(!_i) continue
				case [5,6]
					_i=IsWindowEnabled(w)
					if(action=6) if(_i) continue
					else if(!_i) continue
		else
			sel action
				case [1,4]
				case [0,2,3] continue
				case else end ES_WINDOW
		
		if(&getHwnd) getHwnd=w
		ret i+1
	
	if(wt>0 and GetTickCount-t1>=wt) end "wait timeout"
	wait m_period/1000.0
