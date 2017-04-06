dll user32 [FindWindowW]#FindWindowW2 BSTR'lpClassName @*lpWindowName
dll user32 [FindWindowW]#FindWindowW4 ...

str s="QM_Editor"
zw FindWindowW(@s 0)
zw FindWindowW(@s @"Quick Macros - ok - [Macro776]")
 zw FindWindowW(L"QM_Editor" 0)
zw FindWindowW(@"QM_Editor" 0)
zw FindWindowW2(@"QM_Editor" 0)
zw FindWindowW4(@"QM_Editor" 0)
FindWindow3(@"QM_Editor" 0)
FindWindow4(@"QM_Editor" 0)
