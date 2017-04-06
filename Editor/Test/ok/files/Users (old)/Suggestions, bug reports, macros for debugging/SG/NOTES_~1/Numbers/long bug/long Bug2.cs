 Run macro, read results comments in output
long a b c d e f
int g
a=0x2006000;  b=0x10000008
out "a=0x2006000"
out "b=0x1000008"
out "-----------"
c=b|(a<<32)
d=b+(a<<32)
out ""
out "''c=b|(a<<32)''  is  %i" c=b|(a<<32)
out "''d=b+(a<<32)''  is  %i:			 c, d, do not have correct value"  d=b+(a<<32)
out ""
g=( (b|(a<<32)+1)=0x200600010000009)
out "''g = ( (b|(a<<32)+1) = 0x200600010000009 )''		 gives correct result: g is TRUE: %i" g
out ""
 a=opbefore|(selfbefore<<32)
out "a b c d   		%I64x %I64x %I64x %I64x Wrong out" a b c d

out "a b b|(a<<32) b+(a<<32) %I64x %I64x %I64x %I64x Right out" a b b|(a<<32) b+(a<<32)
out "----------------------------------------------------------------------------------"
a<<32
e=b|a
out "a<<32		e=b|a		f=0x200600010000008"
out ""
f=0x200600010000008
out "a b e f		%I64x %I64x %I64x %I64x Wrong out" a b e f
out "a b b|a 0x...	%I64x %I64x %I64x %I64x Right out" a b b|a 0x200600010000008

out ""
out "Comments: "
out " Shift using ''<<'' with ''long'' type numbers, when using a large shifted number - or, specifically,"
out "	 just assigning any large ''long'' number to a ''long'' variable - results in incorrect"
out "	 value being assigned (OK in direct calculation, or when used directly in output statement,"
out "	 but value gets corrupted when assigned to a variable)"
out " other examples:"
out "-------------------------------------------------------------------------------"
out ""
long y
y=0xFFFFFFFFFFFFFFFF
out "y=0xFFFFFFFFFFFFFFFF	output: %I64x OK" y
y=0xFFFFFFFFFFFFFFF1
out "y=0xFFFFFFFFFFFFFFF1	output: %I64x OK" y
y=0xFFFFFFFFFFFFFF
out "y=0xFFFFFFFFFFFFFF	output: %I64x WRONG" y
y=0x2FFFFFFFFFFFF0
out "y=0x2FFFFFFFFFFFF0	output: %I64x OK" y
y=0x2FFFFFFFFFFFF1
out "y=0x2FFFFFFFFFFFF1	output: %I64x WRONG" y
out ""
out "Corruption seems to start after 0x2FFFFFFFFFFFF0"
out ""
str s="y=0xFFFFFFFFFFFFFFFF	output: ffffffff WRONG[]y=0xFFFFFFFFFFFFFFF1	output: fffffff1 WRONG"
out "One bug has been fixed since QM 2.1.0 dated October 1, 2003, when last output was:"
out s