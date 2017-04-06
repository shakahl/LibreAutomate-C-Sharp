function# $accelMap ;;accelMap: "id keys[]id keys[]..."

 Creates keyboard accelerator table.
 Returns its handle (stored in hacc). Returns 0 if fails or if accelMap is empty.

 accelMap - list of command id and hotkey pairs. Hotkeys are in QM format.

 See also: <MessageLoopOptions>, <DT_SetAccelerators>.


Destroy

ARRAY(ACCEL) a
str s; int i j vk mod
foreach s accelMap
	i=val(s 0 j); if(!j or !i or s[j]!32) continue
	ACCEL ac.cmd=i
	if(!TO_HotkeyFromQmKeys(s+j mod vk)) continue
	ac.key=vk; ac.fVirt=mod&7<<2|1
	a[]=ac

if(a.len) hacc=CreateAcceleratorTable(&a[0] a.len)
ret hacc
