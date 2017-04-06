 str s.all(1024*1024*1024 2) ;;1GB
 str s.getfile("Q:\Downloads\backup-quickmacros.com-7-23-2013.tar.gz")
 out s.len

out
str s
__Stream x.CreateOnFile("Q:\Downloads\quickmac.exe" STGM_READ)
 __Stream x.CreateOnFile("$my qm$\test\test.txt" STGM_READ)
 __Stream x.CreateOnHglobal(0 100000)
 long i1 i2 i3
 PF
 rep 100
	 i1=x.GetSize
 PN
 PO
 out F"{i1} {i2} {i3}"
 
 
 #ret

rep 10
	if(!x.ToStr(s 1000000)) break
	 if(!x.ToStr(s 0 2)) break
	out s.len
	 out x.is.
 IStream_Reset