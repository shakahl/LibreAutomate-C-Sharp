function# ~name

if(!m.IntGet(name _i)) goto ge
ret _i

err+
	 ge
	end "the variable does not exist"
