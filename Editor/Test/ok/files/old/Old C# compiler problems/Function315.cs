 \
function event pid pidparent $name
 event: 1 started, 2 ended, 4 running
out F"{event} {pid} {name}"
 o.o("kkhjh")
 out IsWindow64Bit(pid 1)
 sel name
	 case "csc"
	 case else ret
 
 __ProcessMemory m.Alloc(pid 1000 1)
 
 PROCESS_BASIC_INFORMATION pbi
 if(NtQueryInformationProcess(m.hprocess 0 &pbi sizeof(pbi) 0)) ret
 byte* pp
 m.ReadOther(&pp pbi.PebBaseAddress+16 4)
  out pp
 UNICODE_STRING us
  m.ReadOther(&us pp+0x24 8) ;;current directory
 m.ReadOther(&us pp+0x40 8) ;;command line
  m.ReadOther(&us pp+0x38 8) ;;image path
  out us.Length
 BSTR b.alloc(us.Length)
 m.ReadOther(b.pstr us.Buffer us.Length*2)
 str s=b
 out s
 str ss
 if(findrx(s " @''(.+?)''" 0 0 ss 1)<0) ret
 out ss
 s.getfile(ss)
  outb s s.len 1
 s.get(s 3)
 out s
