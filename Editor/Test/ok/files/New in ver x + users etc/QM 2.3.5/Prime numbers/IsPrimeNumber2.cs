 /
function! x

if(x<=2) ret x=2
if(!(x&1)) ret

int i si=sqrt(x)+1
for i 3 si 2
	if !(x%i)
		 out "%i is divisible by %i" x i
		ret

 out "%i is prime number" x
ret 1
