int nth=getopt(nthreads)
if(nth=1) out; act
if(nth<2) mac "CopyPasteMultiThread"
spe 100
 opt clip 1

int i; str s
for i 0 5
	  paste
	 s.from("(" nth ") " i "[9]aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa[]")
	 s.setsel
	
	 copy
	key HDSE
	out "%i    %s" nth s.getsel

out "ended"
