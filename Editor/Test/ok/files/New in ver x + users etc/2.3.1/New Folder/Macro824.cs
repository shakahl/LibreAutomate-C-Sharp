out
str s
int i
long L
double d

s=""
s=" k"
s="-"
s="56"
s="-56"
s="+56"
s="  - 56"
s="  +  56E"
s="  +  56."
s="  +  056."
s="056k"
s=0xffffffff
s=0x100000005
s="&H5"
s=".67"
s=".4E4"
s="4E-4"
s="123456789012"
s=0xffffffffffffffff
s="0xffffffffffff"
s="0xffffffffffffffff"
s="0xffffffffffffffffffffffff"
s="123456789012345678901"
s="4294967295"
s="4294967299"
s="4294967295.999999"
s="4294967295.9999999"
s="18446744073709551615"
s="18446744073709551616"
s="18446744073709551614.1"
s="18446744073709551615.9"
s="1E19"
s="1E20"
s="0x7fffffffffffffff" ;;9223372036854775807
s="0x8000000000000000"
s="0xffffffffffffffff"
s=" - 000.00078912345E+4"


 s-"-"

out s
out "---------"

i=val(s 0 _i)
out i
out _i

L=val(s 1 _i)
out L
out _i

d=val(s 2 _i)
out d
out _i
 _s.format("%.20e" d); out _s
