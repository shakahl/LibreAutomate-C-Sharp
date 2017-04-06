 Most of this long-type bug has been fixed since October, but one remains

long a = 2147483647
out a + 100 ;; works fine

long b = 21474836470
out b ;; works fine

long c = 2147483648
out c ;; gives negative result		- no, OK now, bug is in QM of Oct 1
out c + 100 ;; gives negative result- no, OK now

 /------------------------------> still to fix
out ""
out "Still a bug here:"
long d = -2147483649
out d 						;; BUG: gives positive result, more in "Long bug1b" macro
 /----------------------------------------------------------
 long f
 for f 2147283648 2147283700
	 out f ;; works fine
out ""
long g 
g = 2147483648
out g ;;range of g = 2147483648 up to g = 4294967295 produces negative value
		;;-OK now
g = 4294967295
out g ;; output -1
		;;OK now