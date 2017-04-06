str images=
 Macro1.bmp
 Macro2.bmp

ARRAY(str) a=images
int i=0
rep
	if(scan(a[i] 0 0 0x1)) break
	i+1; if(i=a.len) i=0; 0.5
