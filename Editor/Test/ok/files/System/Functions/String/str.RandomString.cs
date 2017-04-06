function$ minlen maxlen [$charset]

 Creates random string.
 Returns: self.

 minlen, maxlen - minimal and maximal number of characters that must be in the string.
 charset - characters that must be in the string. Use hyphen to include a range of characters. By default are included characters ASCII 33-126.

 See also: <str.Guid>.

 EXAMPLES
 str s
 s.RandomString(8 8) ;;8 characters ASCII 33-126
 out s
 
 s.RandomString(8 16 "a-zA-Z0-9") ;;8 to 16 alphanumeric characters
 
 s.RandomString(8 8 "[1]-[255]") ;;8 any characters


this.all(RandomInt(minlen maxlen) 2)

int i j c
if(empty(charset))
	for(i 0 this.len) this[i]=RandomInt(33 126)
else
	str s(charset) ss
	 replace hyphens
	for i s.len-2 0 -1
		if(s[i]='-')
			c=s[i+1]-s[i-1]-1
			if(c<0) continue
			ss.all(c 2)
			c=s[i-1]
			for(j 0 ss.len) c+1; ss[j]=c
			s.replace(ss i 1)
			i-2
	 find min and max char
	int minchar(255) maxchar(1)
	for(i 0 s.len)
		if(s[i]<minchar) minchar=s[i]
		if(s[i]>maxchar) maxchar=s[i]
	 generate random chars between min and max, and reject chars not in charset
	for(i 0 this.len)
		rep
			c=RandomInt(minchar maxchar)
			if(findc(s c)>=0) this[i]=c; break
ret this
