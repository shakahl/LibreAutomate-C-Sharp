out

str f="$qm$\winapi2.txt"
 str f="$desktop$\test.txt"

Q &q
str sd.getfile(f)
str ss.encrypt(10 sd)
out ss

Q &qq
str sss.encrypt(10 f "" 0x100)
out sss

Q &qqq
str sd2.getfile(f)
out Crc32(sd2 sd2.len)

Q &qqqq
out Crc32(f 0 1)
 out GetLastError
Q &qqqqq
outq
