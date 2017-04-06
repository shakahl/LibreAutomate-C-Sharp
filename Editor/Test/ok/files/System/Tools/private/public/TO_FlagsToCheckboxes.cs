 /
function f ~&s1 [~&s2] [~&s4] [~&s8] [~&s16] [~&s32] [~&s64] [~&s128] [~&s0x100] [~&s0x200] [~&s0x400] [~&s0x800] [~&s0x1000] [~&s0x2000] [~&s0x4000] [~&s0x8000]

int i; str** p=+(&f+4)

for i 0 16
	str& s=p[i]
	if(f&(1<<i) and &s) s=1
