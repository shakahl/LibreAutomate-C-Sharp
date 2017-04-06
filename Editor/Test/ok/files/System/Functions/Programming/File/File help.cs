 Functions of File class can be used to read and write files.
 A variable of this class represents an open file.

 In most cases, you don't have to use this class to read/write files. It is easier to load file into a str variable (<help>str.getfile</help>), modify the variable, and write it to file (str.setfile).

 This class wraps C file functions, such as fopen.
 Does not support files bigger than 2 GB.
 Instead you can use Windows file functions (documented in MSDN). To open and autoclose, use class __HFile.


 EXAMPLE
File f.Open("$desktop$\test QM File class.txt" "w+")
err mes- "failed to create file"
f.Write("line1[]line2[]line3[]")
f.SetPos(0)
str s
rep
	if(!f.ReadLine(s)) break
	out s
