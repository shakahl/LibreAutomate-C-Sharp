def WM_INPUT 0x00FF
def RIDI_PREPARSEDDATA 0x20000005
def RIDI_DEVICENAME 0x20000007
def RIDI_DEVICEINFO 0x2000000b
def RIDEV_REMOVE 0x00000001
def RIDEV_EXCLUDE 0x00000010
def RIDEV_PAGEONLY 0x00000020
def RIDEV_NOLEGACY 0x00000030
def RIDEV_INPUTSINK 0x00000100
def RIDEV_CAPTUREMOUSE 0x00000200
def RIDEV_NOHOTKEYS 0x00000200
def RIDEV_APPKEYS 0x00000400
def RIDEV_EXMODEMASK 0x000000F0
def RID_INPUT 0x10000003
def RID_HEADER 0x10000005
def RIM_TYPEMOUSE 0
def RIM_TYPEKEYBOARD 1
def RIM_TYPEHID 2
def RI_KEY_MAKE 0
def RI_KEY_BREAK 1
def RI_KEY_E0 2
def RI_KEY_E1 4
def RI_KEY_TERMSRV_SET_LED 8
def RI_KEY_TERMSRV_SHADOW 0x10
def RIM_INPUT 0
def RIM_INPUTSINK 1
type RAWINPUTDEVICELIST hDevice dwType
type RAWINPUTDEVICE @usUsagePage @usUsage dwFlags hwndTarget
type RAWINPUTHEADER dwType dwSize hDevice wParam
type RAWMOUSE @usFlags ulButtons []@usButtonFlags [+2]@usButtonData ulRawButtons lLastX lLastY ulExtraInformation
type RAWKEYBOARD @MakeCode @Flags @Reserved @VKey Message ExtraInformation
type RAWHID dwSizeHid dwCount !bRawData
type RAWINPUT RAWINPUTHEADER'header RAWMOUSE'mouse []RAWKEYBOARD'keyboard []RAWHID'hid
dll user32
	#GetRawInputDeviceList RAWINPUTDEVICELIST*pRawInputDeviceList *puiNumDevices cbSize
	#GetRawInputDeviceInfo hDevice uiCommand !*pData *pcbSize
	#RegisterRawInputDevices RAWINPUTDEVICE*pRawInputDevices uiNumDevices cbSize
	#GetRawInputData hRawInput uiCommand !*pData *pcbSize cbSizeHeader


ARRAY(RAWINPUTDEVICELIST) a.create(10)
int i kid nDev bSize; str s devName devType
i=10*sizeof(RAWINPUTDEVICELIST)
nDev=GetRawInputDeviceList(&a[0] &i sizeof(RAWINPUTDEVICELIST))
for i 0 nDev
	sel a[i].dwType
		case 0 devType="Mouse"
		case 1 devType="Keyboard"
		case else continue
	GetRawInputDeviceInfo(a[i].hDevice RIDI_DEVICENAME 0 &bSize)
	GetRawInputDeviceInfo(a[i].hDevice RIDI_DEVICENAME devName.all(bSize) &bSize)
	devName.fix
	memcpy &kid s.encrypt(2 devName) 4 ;;make numeric id
	
	out "%s %i %s" devType kid devName
	
