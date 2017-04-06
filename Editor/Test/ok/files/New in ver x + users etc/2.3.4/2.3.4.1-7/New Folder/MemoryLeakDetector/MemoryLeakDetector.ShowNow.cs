___MLD y
GetAll(y)

y.PageFile-x.PageFile
y.WorkingSet-x.WorkingSet
y.PagedPool-x.PagedPool
y.NonPagedPool-x.NonPagedPool
y.nKernel-x.nKernel
y.nGdi-x.nGdi
y.nUser-x.nUser
y.Heap-x.Heap
y.nHeapAllocations-x.nHeapAllocations

str s

 s.format("Committed %i, WorkingSet %i, Heaps %i, # alloc %i, PP %i, NPP %i,   Kernel %i, GDI %i, User %i" y.PageFile y.WorkingSet y.Heap y.nHeapAllocations y.PagedPool y.NonPagedPool y.nKernel y.nGdi y.nUser)

s.format("Committed %i, Heaps %i, # alloc %i,   Kernel %i, GDI %i, User %i" y.PageFile y.Heap y.nHeapAllocations y.nKernel y.nGdi y.nUser)

out s
