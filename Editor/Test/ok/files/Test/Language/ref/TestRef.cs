typelib Wsh {F935DC20-1CF0-11D0-ADB9-00C04FD58A0B} 1.0
typelib MailBee "MailBee.dll"
dll kernel32 #ReplaceFile $lpReplacedFileName $lpReplacementFileName $lpBackupFileName dwReplaceFlags !*lpExclude !*lpReserved
dll kernel32
	#SetFilePointer hFile lDistanceToMove *lpDistanceToMoveHigh dwMoveMethod
	#SetFileValidData hFile %ValidDataLength
def AJO 5
type QWERTY
	a
	b
interface Iunk :IUnknown a b
interface Iunk2 :IUnknown
	a(n)
	b()
class CQWERTY
	a
	b
category CAT : Wsh QWERTY
category CATT : CAT
typelib InetCtlsObjects {48E59290-9880-11CF-9754-00AA00C00908} 1.0
