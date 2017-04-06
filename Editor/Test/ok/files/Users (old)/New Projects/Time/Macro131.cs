dll kernel32 GetSystemTimeAsFileTime FILETIME*lpSystemTimeAsFileTime
long t
GetSystemTimeAsFileTime +&t
t/10000000 ;;the number of seconds since January 1, 1601 (UTC)
 out t

 to get a number that would fit into an int variable (approx +- 2000000000), subtract eg 12000000000
int tt=t-12000000000
out tt
