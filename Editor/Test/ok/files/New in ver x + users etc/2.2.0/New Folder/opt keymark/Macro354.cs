out
dll "qm.exe" IsExtendedKey !vk

IStringMap m=CreateStringMap(0)
int i sc; str s ss sss k
for i 1 256
	sc=MapVirtualKey(i 0)
	s.fix(0); FormatKeyString(i 0 &s)
	if(!sc and matchw(s "(*)")) continue
	
	 out "%i %i %s" i sc s
	k=sc; if(IsExtendedKey(i)) k+"e";; out s
	sss.format("%i %s %s" i k s)
	if(m.Get2(k ss)) out "%s (%s)" sss ss
	else m.Add(k sss)
	