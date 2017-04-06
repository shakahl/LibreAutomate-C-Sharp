 /
function! x

if(x<2) ret

int i si=sqrt(x)+1
for i 2 si
	if !(x%i)
		 out "%i is divisible by %i" x i
		ret

 out "%i is prime number" x
ret 1
