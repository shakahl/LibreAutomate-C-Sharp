 /
function!* resId [&resSize] [resType]

 Finds a resource in exe and returns pointer to raw data.
 The pointer remains valid while the exe runs.
 The data does not consume additional memory and you don't have to free it.
 The data is read-only. If you need to modify it, copy it to a str variable.

 idRes - resource id or name.
 resSize - if used and not 0, receives resource size.
 resType - resource type. Default or 0: RT_RCDATA (10). Note: resource editors may interpret RT_RCDATA as string, so it is better to specify resource type 10 when adding.

 EXAMPLE
 str s; int n
 byte* b=ExeLoadDataResource(100 n)
 if(!b) ret
 s.fromn(b n)
 out s


dll kernel32 [FindResourceA]#FindResource hModule $lpName $lpType
dll kernel32 #LoadResource hModule hResInfo
dll kernel32 !*LockResource hResData
dll kernel32 #SizeofResource hModule hResInfo

int hModule=GetExeResHandle
if(&resSize) resSize=0
if(!resType) resType=10

int hRes=FindResource(hModule +resId +resType); if(!hRes) ret
int hGlob=LoadResource(hModule hRes); if(!hGlob) ret
byte* lpData=LockResource(hGlob); if(!lpData) ret
if(&resSize) resSize=SizeofResource(hModule hRes)
ret lpData
