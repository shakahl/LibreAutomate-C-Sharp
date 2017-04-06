out
dll "qm.exe" #Rx2Find $s $rx [flags] [&length]
ref __pcre2

int test=1
str s rx
sel test
	case 0
	s="one -12 two"
	rx="\d+"
	
	case 1
	s.getmacro("ShowDialog")
	rx="[\-!~]([k-z]+|\d+)\b"
	
	case 2
	s.getmacro("TO_Window")
	rx="(?m)^[ ;]*BEGIN DIALOG *[][ ;]*((?:.+[])+)[ ;]*END DIALOG *(?:[])?(?:[ ;]*DIALOG EDITOR: +([^[]]+)(?:[])?)?"

WakeCPU

 __testq__
PF
rep 3
	rep 1
		int k1 i1=findrx(s rx 0 0 k1)
	 int k1 i1=findrx(s rx 0 0x80000 k1)
	PN
PO
out "---"
PF
rep 3
	PF
	int k2 i2=Rx2Find(s rx 0 k2)
	 int k2 i2=Rx2Find(s rx 0x1000 k2)
	 int k2 i2=Rx2Find_(s rx 0 k2)
	 int k2 i2=Rx2Find_(s rx PCRE2_UTF k2) ;;slower 50%
	 int k2 i2=Rx2Find_(s rx PCRE2_UTF|PCRE2_NO_UTF_CHECK k2) ;;not faster
	PN
	PO
	 0.01
	 Rx2Find(s rx2 0 k2);PN
 PO

out "---"
out F"{i1} {k1}"
out F"{i2} {k2}"

 speed: 16  14  
 speed: 12  17  

 speed: 16  14  
 speed: 1  9  
