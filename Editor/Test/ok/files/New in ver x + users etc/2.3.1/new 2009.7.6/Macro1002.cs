0053B450 B9 08 80 03 00   mov         ecx,38008h 
	//__asm mov ecx, 30008h //MAKELONG(argsize|(isStdcall<<15), id)
	__asm jmp ManagedCallback
0053B455 E9 F6 FE FF FF   jmp         ManagedCallback (53B350h) 

