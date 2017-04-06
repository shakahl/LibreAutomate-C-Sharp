 /
function! x

int i
for i 2 x
	if x%i=0
		 out "%i is divisible by %i" x i
		break

if i=x
	 out "%i is prime number" x
	ret 1
