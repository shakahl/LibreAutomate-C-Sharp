 key a
 key (VK_OEM_5)a

 opt keychar 1
 key "a"
 key "`a"

outp "a"

 key (0x5dc|0x40000)
 key (0x5dc)
 key (0x3A3|0x40000)

 key CE
 spe 1
 int i
 for i 256 512
	 _s=i
	 key (_s) V (i) V (i+256) Y

 BlockInput 1
 key a (5.0) b
 BlockInput 0

 key a/b

 key+ (0x30000|0x38)
 key e
 key- (0x30000|0x38)
 key (0x30000|0x38) (0x30000|0x1D) (0x30000|0x36) m
 key (0xA0)m

 key (0x3A3|0x40000)
#ret

