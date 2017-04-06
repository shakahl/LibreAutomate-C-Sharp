dll- "nodll.dll" FuncName a b
dll- "user32" FuncName2 a b
dll- "user32" [aliass]FuncName3 a b
dll- "user32" [3]FuncName4 a b
dll- "user32" [3333]FuncName5 a b

 out &FuncName
 out &FuncName2
 FuncName2 0 0
 out &FuncName3
 out &FuncName4
out &FuncName5
 err
out 1

 test alias and ordinal
