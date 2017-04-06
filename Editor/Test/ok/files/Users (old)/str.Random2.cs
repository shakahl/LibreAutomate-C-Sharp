function minlen maxlen [minchar] [maxchar]

 Creates random string.

 minlen, maxlen - minimal and maximal number of characters that must be in the string.
 minchar, maxchar - range of characters that must be in the string. For example, 'A' and 'Z'. By default are included all characters between ASCII 33 and 127. This does not include space characters.

 EXAMPLE
 str s
 s.Random(5 5)
 out s


if(!minchar) minchar=33
if(!maxchar) maxchar=127

this.all(Uniform(minlen maxlen) 2)
for(_i 0 this.len) this[_i]=Uniform(minchar maxchar)
