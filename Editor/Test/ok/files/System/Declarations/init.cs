 Function init runs when QM starts or file is opened.
 It initializes QM extensions.
 Also it calls user's function init2, if exists.

def __QM_INIT_BEGIN 1

 Windows declarations
#compile "WinConstants"
#compile "VirtualKeys"
#compile "WinStyles"
#compile "WinMessages"
#compile "WinTypes"
#compile "WinInterfaces"
#compile "WinFunctions"
#compile "MSVCRT"

 typelibs and refs
#compile "References"

 QM declarations
#compile "QmDef"
#compile "QmDll"
#compile "Classes"
#compile "Categories"

 some spec variables
#opt nowarnings 1
__IQmFile+ _qmfile=__GetQmFile
#opt nowarnings 0

 some QM window classes
TO_InitToolsControl

def __QM_INIT_END 1

 call user function init2, if exists and no errors
#opt err 1
init2
#opt err 0
