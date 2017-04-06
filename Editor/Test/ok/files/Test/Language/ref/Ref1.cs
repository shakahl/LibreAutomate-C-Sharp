def ABBA WINAPI.MCHT_NEXT
def ABBA2 LVN_GETDISPINFOW
def LVN_GETDISPINFOW (LVN_FIRST-77)
def OOP ABBA
 def LVN_FIRST 50
def LVN_NEXT LVN_FIRST

type TABC a b c
type TIC a TABC'b c

dll kernel32 #ResumeThread hThread
type DCB DCBlength BaudRate fBitFields @wReserved @XonLim @XoffLim !ByteSize !Parity !StopBits !XonChar !XoffChar !ErrorChar !EofChar !EvtChar @wReserved1
type COMMCONFIG dwSize @wVersion @wReserved DCB'dcbx dwProviderSubType dwProviderOffset dwProviderSize !wcProviderData
 type COMMCONFIG dwSize @wVersion @wReserved WINAPI.DCB'dcbx dwProviderSubType dwProviderOffset dwProviderSize !wcProviderData
 dll kernel32 #CommConfigDialog $lpszName hWnd COMMCONFIG*lpCC

def C100 10
def C101 20
def C102 30
type C1 a b
type C2 C1'a b
type C3 C2'a b
type C4 C3'a b[C100]
 type C4 C3'a b
type C5 C4'a b
type C6 C5'a b C4'c
type C7 C6'a b[C101]
type C8 C7'a b[C102]
 type C7 C6'a b
 type C8 C7'a b
type C9 C8'a b
dll kernel32 #CommConfigDialog $lpszName hWnd C9*lpCC


