 /
function hlv $s0 w0 [$s1] [w1] [$s2] [w2] [$s3] [w3] [$s4] [w4] [$s5] [w5] [$s6] [w6] [$s7] [w7] [$s8] [w8] [$s9] [w9] [$s10] [w10] [$s11] [w11] [$s12] [w12] [$s13] [w13] [$s14] [w14]

 Adds columns to SysListView32 control that has LVS_REPORT style (1).
 To create control with this style, specify 0x54000001 style in dialog definition.

 s0-s14 - column text.
 w0-w14 - column width. Negative is interpreted as -percentage.


int i
lpstr* sp=&s0
int* wp=&w0
for i 0 getopt(nargs)-2 2
	TO_LvAddCol hlv -1 sp[i] wp[i]
