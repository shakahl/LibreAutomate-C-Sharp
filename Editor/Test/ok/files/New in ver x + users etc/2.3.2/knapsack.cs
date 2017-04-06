 Before:
 Edit the downloaded hs.c file:
    Before each function that you'll need, insert this line: __declspec(dllexport)
    These functions are knapsack_sort knapsack_hs knapsack_restore.

 Then run this macro. It creates dll on desktop.

UnloadDll("$desktop$\knapsack.dll")
__Tcc x.Compile(":Q:\Downloads\hs.c" "$desktop$\knapsack.dll" 2)

 Then you can use the dll.
 Function declarations:
dll- "$desktop$\knapsack.dll"
	#knapsack_hs n *w c *p *z *x
	#knapsack_sort *a *b n *perm
	#knapsack_restore *x n *perm

 Or you can create code in memory instead of creating dll.
