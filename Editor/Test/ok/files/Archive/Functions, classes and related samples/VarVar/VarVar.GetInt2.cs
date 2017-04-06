function# ~namePart1 ~namePart2

if(!m.IntGet(_s.from(namePart1 namePart2) _i)) goto ge
ret _i

err+
	 ge
	end "the variable does not exist"
