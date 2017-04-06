 several useful functions from msvcrt.dll

dll msvcrt
	 Character
	#isalpha char
	#isxdigit char ;;Hexadecimal digit
	#islower char
	#isupper char
	#tolower char
	#toupper char
	 Memory
	!*malloc n
	!*realloc !*memblock n
	!*calloc n size
	free !*memblock
	!*memmove !*to !*from n ;;can overlap
	!*memcpy !*to !*from n ;;cannot overlap
	#memcmp !*p1 !*p2 n ;;for case insensitive, use StrCompare instead
	!*memset !*dest char count
	!*memchr !*buf c count
	_swab $src $dest n
	 String
	#strcmp $s1 $s2 ;;result: 0 s1=s2, 1 s1>s2, -1 s1<s2
	#strncmp $s1 $s2 n
	$strstr $s1 $s2
	$strchr $s1 ch
	#strspn $string $strCharSet
	$setlocale Category $Locale
	[_strrev]$strrev $string
	 File
	#_wfopen @*filename @*mode ;;mode: "rb" read, "wb" write new, "ab" append, "r+b" read/write existing, "w+b" read/write new, "a+b" read/append
	#fclose file
	#fflush file ;;write to disk (without closing)
	#fread $buffer itemsize count file ;;read binary data, itemsize*count bytes
	#fwrite $buffer itemsize count file ;;write binary data, itemsize*count bytes
	$fgets $string nchars file ;;read string; string must have allocated min nchars bytes
	#fputs $string file ;;write string
	#fgetc file ;;read character
	#fputc char file ;;write character
	#fprintf file $formatstring ... ;;write formatted string
	#fscanf file $formatstring ... ;;read formatted string
	#fgetpos file int*pos ;;get binary position
	#fsetpos file int*pos ;;set binary position
	#ftell file ;;return text position
	#fseek file offset origin ;;set text position; origin: 0 from beginning, 1 from current, 2 from end
	#feof file ;;test for end-of-file
	#_filelength handle
	#_fileno file
	 Math
	^pow ^x ^y ;;Returns x raised to the power of y
	^fmod ^x ^y ;;Returns floating-point remainder of x/y. To get integer remainder use operator % instead.
	^sqrt ^x ;;Square root
	^modf ^x ^*intptr ;;Returns fractional part of x, and stores integer part of x into a double variable whose address is intptr
	^exp ^x ;;Exponential
	^log ^x ;;Natural logarithm. To get logarithm of any base: log(x)/log(base).
	^log10 ^x ;;Base-10 logarithm
	^cos ^x ;;Cosine
	^sin ^x ;;Sine
	^tan ^x ;;Tangent
	^acos ^x ;;Arccosine
	^asin ^x ;;Arcsine
	^atan ^x ;;Arctangent
	^ceil ^x ;;Returns integer value that is >= x, eg 6 if x is 5.2, and -5 if x is -5.2. See also: Round.
	^floor ^x ;;Returns integer value that is <= x, eg 5 if x is 5.2, and -6 if x is -5.2. See also: Round.
	#abs x ;;Returns integer without sign, eg 5 if x is 5 or -5
	^fabs ^x ;;Returns double without sign, eg 5.2 if x is 5.2 or -5.2
	^_hypot ^x ^y ;;Hypotenuse. Same as sqrt(pow(x 2)+pow(y 2)).
	 Sort
	qsort !*base num width fcompare ;;fcompare must be function[c]# typename*a typename*b
	 Errors
	#*_errno

def errno *_errno()
