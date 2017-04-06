 /
function bOpenDrive [cDrive]

 From http://www.codeguru.com/Cpp/W-P/system/cd-rom/article.php/c5775/

 Open or Close CD Drive
 cDrive is Drive Letter to Open, or 0x01 for 'Default' drive

 EXAMPLES
 CD_OpenCloseDrive(1)    ;;Open First Logical CD Door

 CD_OpenCloseDrive(1, 'G')  ;;Open CD Door for Drive G

 CD_OpenCloseDrive(0, 'G') ;;Close CD Door for Drive G


def MCI_DEVTYPE_CD_AUDIO  516
def MCI_OPEN_TYPE    0x2000
def MCI_OPEN_TYPE_ID  0x1000
def MCI_OPEN_ELEMENT  0x200
def MCI_OPEN_SHAREABLE  0x100
def MCI_OPEN         0x803
def MCI_STATUS_READY  0x7
def MCI_SET          0x80D
def MCI_SET_DOOR_OPEN  0x100
def MCI_SET_DOOR_CLOSED  0x200
def MCI_CLOSE        0x804
def MCI_WAIT         0x2
type MCI_OPEN_PARMS dwCallback wDeviceID $lpstrDeviceType $lpstrElementName $lpstrAlias
type MCI_STATUS_PARMS dwCallback dwReturn dwItem @dwTrack
dll winmm #mciSendCommand wDeviceID uMessage dwParam1 dwParam2

MCI_OPEN_PARMS op.lpstrDeviceType = +MCI_DEVTYPE_CD_AUDIO
MCI_STATUS_PARMS st
int flags
str szDriveName="X:"

if(cDrive > 1)
	szDriveName[0] = cDrive
	op.lpstrElementName = szDriveName;
	flags = MCI_OPEN_TYPE | MCI_OPEN_TYPE_ID | MCI_OPEN_ELEMENT | MCI_OPEN_SHAREABLE
else flags = MCI_OPEN_TYPE | MCI_OPEN_TYPE_ID | MCI_OPEN_SHAREABLE

if(!mciSendCommand(0 MCI_OPEN flags &op))
	st.dwItem = MCI_STATUS_READY
	
	if(bOpenDrive) mciSendCommand(op.wDeviceID MCI_SET MCI_SET_DOOR_OPEN 0)
	else mciSendCommand(op.wDeviceID MCI_SET MCI_SET_DOOR_CLOSED 0)
	
	mciSendCommand(op.wDeviceID MCI_CLOSE MCI_WAIT 0)
