function str&code [~&userVar]

 Replaces variable declaration template in code with real declaration or just variable name.
 Particularly, replaces "{decl}" (beginning of a line) with "Type var" or "var". Calls VD(F"-r decl" userVar) on self.
 {decl} must be like {Acc a}, without options.

ARRAY(str) a
int i=findrx(code "^\{(.+?)\}" 0 8 a); if(i<0) ret
code.replace(VD(_s.from("-r " a[1]) userVar) i a[0].len)
