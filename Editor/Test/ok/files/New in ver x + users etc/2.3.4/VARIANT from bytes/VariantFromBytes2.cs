 /
function $bytesHex VARIANT&v

str s.decrypt(8 bytesHex)
BSTR b.alloc(s.len)
int i
for(i 0 s.len) b[i]=s[i]
v.attach(b)
