Normally should not grow. On my PC QM often runs several days, and memory usage never grows above 20 MB.

Maybe some macros allocate and don't release memory. QM functions and variables (str, ARRAY, COM interface and other) release memory automatically. With some functions, eg Windows API functions, need to explicitly release memory, close handle, etc.

Of course can be a bug in some QM function too. Or in a dll or COM component.

If you have many macros, it is difficult to find memory leaks. I'll try to create a memory leak detector for QM.

If you suspect some macros, try to run them in separate process, if possible.
