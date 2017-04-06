 /
function $bytesHex VARIANT&v

 Stores binary data into VARIANT variable.

 bytesHex - binary data as hex string.
 v - variable that receives the data as ARRAY(byte).

 EXAMPLE
 VARIANT d
 VariantFromBytes "52 80 00 d2" d ;;store binary data of 4-bytes length


str s.decrypt(8 bytesHex)
ARRAY(byte) a.create(s.len)
memcpy &a[0] s s.len
v.attach(a)
