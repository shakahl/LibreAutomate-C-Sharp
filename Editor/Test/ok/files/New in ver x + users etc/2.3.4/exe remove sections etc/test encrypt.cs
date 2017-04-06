out

 str s.getmacro("ShowDialog")
 out Crc32(s s.len)
 s.encrypt(32)
 out Crc32(s s.len)
 out s
 s.decrypt(32)
 out Crc32(s s.len)
 out s


 str s.getfile("upx.exe")
  out Crc32(s s.len)
 Q &q
 rep(10) s.encrypt(32)
  out Crc32(s s.len)
 rep(10) s.decrypt(32)
 Q &qq
 outq
 out Crc32(s s.len)

ARRAY(str) ap ad
GetFilesInFolder ap "$qm$" "*.ico"
ad.create(ap.len)
int i
for i 0 ap.len
	ad[i].getfile(ap[i])
	 out ap[i]
	 out ad[i].len

Q &q
for i 0 ad.len
	ad[i].encrypt(32)
Q &qq
outq
