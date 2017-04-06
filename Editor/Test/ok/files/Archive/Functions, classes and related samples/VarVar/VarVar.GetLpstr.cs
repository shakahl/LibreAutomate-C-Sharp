function$ ~name

lpstr r=m.Get(name)
if(!r) goto ge
ret r

err+
	 ge
	end "the variable does not exist"
