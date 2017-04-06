 NOTE: QM 2.1.8 has "file modified" trigger, so this is only for older versions

str f="$desktop$\test.txt"
DATE dcur dprev
Dir d
rep
	if(!d.dir(f)) break
	dcur=d.TimeModified2
	if(dprev and dcur!=dprev)
		bee
		out dprev
		out dcur
		 mac "..."
	dprev=dcur
	1
