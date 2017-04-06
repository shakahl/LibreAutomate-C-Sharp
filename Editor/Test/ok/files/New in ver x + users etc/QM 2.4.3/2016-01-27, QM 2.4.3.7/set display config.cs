def SDC_TOPOLOGY_INTERNAL 0x00000001
def SDC_TOPOLOGY_EXTERNAL 0x00000008
def SDC_TOPOLOGY_CLONE 0x00000002
def SDC_TOPOLOGY_EXTEND 0x00000004
def SDC_APPLY 0x00000080
dll user32 [SetDisplayConfig]#SetDisplayConfig_58912 numPathArrayElements !*pathArray numModeInfoArrayElements !*modeInfoArray flags

int f
sel ListDialog("Internal[]External[]Clone[]Extend")
	case 1 f=SDC_TOPOLOGY_INTERNAL
	case 2 f=SDC_TOPOLOGY_EXTERNAL
	case 3 f=SDC_TOPOLOGY_CLONE
	case 4 f=SDC_TOPOLOGY_EXTEND
	case else ret

int R=SetDisplayConfig_58912(0 0 0 0 f|SDC_APPLY)
if(R) out F"failed: {R}"
