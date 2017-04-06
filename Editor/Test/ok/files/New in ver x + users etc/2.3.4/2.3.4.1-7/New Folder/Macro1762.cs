str data="message"
str password="password"
str s
s.HMAC_SHA1(data password 1)
 outb s s.len
out s
