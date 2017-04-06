long nfree
GetDiskFreeSpaceEx("c:" &nfree 0 0)
long kb=nfree/1024
double mb=nfree/pow(2 20)
out "bytes %I64i, KB %I64i, MB %g" nfree kb mb
