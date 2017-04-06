 /test CsScript memory
str code.getmacro("cs1")
code.findreplace("1000" _s.RandomString(8 8 "1-9"))

PF
CsScript x.Init
PN
x.AddCode(code)
 x.x.AddCodeMono(code)
PN
rep 5
	_i=x.Call("Test.Add" 1 2)
	PN
 PO
 out _i

 In reality Mono does not save memory, but just makes worse.
 Mono itself needs much memory. Without Mono initial memory is 14 MB. With Mono 27 MB.
 With Mono, memory grows anyway. Tested compiling 100 different (almost identical) scripts. Normally adds 4-5 MB. With Mono - 3 MB.
