 /Menu icons test
function! $map $icons flags

if m_hh
	Delete
else
	m_hh=SetWindowsHookEx(WH_CBT &__MenuIcons_HookCbt 0 GetCurrentThreadId)

int i j hi; POINT p; str s
rep
	p.x=val(map 0 j); if(!j) ret
	map+j
	if(map[0]='=') map+1; else ret
	p.y=val(map 0 j); if(!j) ret
	map+j
	m_map[]=p
	if(!map[0]) break

sel flags&3
	case 0 ;;list of icons
	m_il=ImageList_Create(16 16 ILC_MASK|ILC_COLOR32 0 8)
	j=0
	foreach s icons
		hi=GetFileIcon(s)
		if(hi)
			if(ImageList_ReplaceIcon(m_il -1 hi)!=j) ret
			DestroyIcon hi
			j+1
		else
			for i m_map.len-1 -1 -1
				if(m_map[i].y=j) m_map.remove(i)
				else if(m_map[i].y>j) m_map[i].y-1
	
	case 1 ;;imagelist as bmp
	m_il=__ImageListLoad(icons)
	
	case 2 ;;imagelist as handle
	m_il=icons

ret m_il!0
