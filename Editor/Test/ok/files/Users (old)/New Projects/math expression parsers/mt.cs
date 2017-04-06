typelib MTPARSERCOMLib "C:\Documents and Settings\G\Desktop\MTParserCOM.dll"
MTPARSERCOMLib.MTParser m._create
 MTPARSERCOMLib.sMTOperator op.description
 MTPARSERCOMLib.sMTConstant v
 MTPARSERCOMLib.sMTFunction f
 MTPARSERCOMLib.sMTSyntax s

BSTR em
m.defineConst("CONST" 2)
m.defineMacro("ADD(a,b)" "a+b" em)
 m.setSyntax

int i=m.evaluate("ADD(1, CONST)")
out i

