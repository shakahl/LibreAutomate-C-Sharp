 /
function [flags] ;;flags: 1 right-align

 Removes unnecessary spaces from text table columns.

 EXAMPLE
 str s=
  AAAAAAAAA     BBBBBBBBBB     CCCCCCCCC
     1111          2222           33
     11111         22222          333
 
 s.CompactTextTable(1)
 out s


ARRAY(str) ar ac af ;;rows, columns, format
ARRAY(int) aw ;;column widths

ar=this

int i j k nc

 column widths
for i 0 ar.len
	k=tok(ar[i] ac -1 " ")
	if(i=0) nc=k; aw.create(nc); else if(k!=nc) end ES_BADARG
	for j 0 nc
		k=ac[j].len
		if(k>aw[j]) aw[j]=k

 format
af.create(nc)
str f=iif(flags&1 "%%%is " "%%-%is ")
for(i 0 nc) af[i].format(f aw[i])
af[i-1].set("[]" (af[i-1].len-1))

 create table
this=""
for i 0 ar.len
	tok(ar[i] ac -1 " ")
	for(j 0 nc) this.formata(af[j] ac[j])
