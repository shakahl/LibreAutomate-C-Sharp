 Does not work

int vk=wait(0 K)
str ks.all(256)
GetKeyboardState(ks)
out ks[16]
ks[vk]=128
word char
if(ToAscii(vk MapVirtualKey(vk 0) ks &char GetMod&4!=0))
	out char
out GetMod
