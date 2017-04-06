 int* p
 int p
 str p
 str p=""
 str p="absd"; p[2]=0
 str p="absd"; p[0]=0
 lpstr p
 lpstr p=""
 lpstr p="hhh"
 BSTR p
 BSTR p=""
 BSTR p="kkkk"; p[2]=0
 BSTR p="kkkk"; p[0]=0
 word* p
 word* p=L"asd"
 word* p=@"asd"
 word* p=@"asd"; p[0]=0
 VARIANT p
 VARIANT p=9
 VARIANT p=0
 VARIANT p=""
 VARIANT p="asdf"
 VARIANT p="asdf"; p.bstrVal[2]=0
 VARIANT p="asdf"; p.bstrVal[0]=0

 out len(p)
out empty(p)
