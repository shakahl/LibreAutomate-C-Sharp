int i; long lo
i=0xffffffff
lo=i; out lo ;;-1
lo=ConvertSignedUnsigned(i); out lo ;;4294967295

word w; int j
w=0xffff
j=w; out j ;;65535
j=ConvertSignedUnsigned(w 2); out j ;;-1

byte c; int k
c=0xff
k=c; out k ;;255
k=ConvertSignedUnsigned(c 1); out k ;;-1
