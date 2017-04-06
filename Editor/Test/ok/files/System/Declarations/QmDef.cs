 PUBLIC

def __FUNCTION__ getopt(itemname)
def AUTODELAY -2 ;;use with wait
def TRUE 0xFFFF ;;use with COM functions

 PCRE flags for findrx and replacerx
def RX_CASELESS 0x100 ;;same as flag 1
def RX_MULTILINE 0x200 ;;same as flag 8
def RX_DOTALL 0x400
def RX_EXTENDED 0x800
def RX_ANCHORED 0x1000
def RX_DOLLAR_ENDONLY 0x2000
def RX_EXTRA 0x4000
def RX_NOTBOL 0x8000
def RX_NOTEOL 0x10000
def RX_UNGREEDY 0x20000
def RX_NOTEMPTY 0x40000
def RX_NO_AUTO_CAPTURE 0x100000

 for GetQmItemsInFolder
type QMITEMIDLEVEL @id @level

 PRIVATE

#opt hidedecl 1

type STRINT ~s i
type LPSTRINT $s i
type INTLPSTR i $s
type COMPAREFIF ~f1 ~f2 flags ;;flags: 1 f1 newer, 2 f2 newer, 4 f1 missing, 8 f2 missing, 0x100 equal

def __ME_W1 "all or part of features of this function are unavailable in exe"
def __ME_W2 "if using flag 1, may need #exe addtextof" ;;fbc (phpexec etc).
def __S_RX_DD "(?m)^[ ;]*BEGIN DIALOG *[]((.+[])+)[ ;]*END DIALOG *$"
def __S_RX_DDDE "(?m)^[ ;]*BEGIN DIALOG *[][ ;]*((?:.+[])+)[ ;]*END DIALOG *(?:[])?(?:[ ;]*DIALOG EDITOR: +([^[]]+)(?:[])?)?"
def __S_RX_MD "(?ms)^[ ;]*BEGIN MENU *[](.*?)^[ ;]*END MENU *$"
def __S_FILE_ATTR "READONLY[]HIDDEN[]SYSTEM[]DIRECTORY[]ARCHIVE[]ENCRYPTED[]NORMAL[]TEMPORARY[]SPARSE_FILE[]REPARSE_POINT[]COMPRESSED[]OFFLINE[]NOT_CONTENT_INDEXED"

type __REGEMAILACCOUNT ~name ~keyname
def RK_IAM "Software\Microsoft\Internet Account Manager"
def RK_IAMA "Software\Microsoft\Internet Account Manager\Accounts"

 number of ticks in time span
def DT_DAY 864000000000L
def DT_HOUR 36000000000L
def DT_MINUTE 600000000L
def DT_SECOND 10000000L
def DT_MS 10000L
def DT_MCS 10L

 fbc
#opt obsoletedecl 1
def ES_INIT ERR_INIT
def ES_BADARG "1002 incorrect argument"
def ES_FAILED "1004 failed"
def ES_WINDOW "1005 Window or child window not found."
def ES_OBJECT "1006 Object not found."
def ES_WSH ERR_WSH
def ES_OUTOFMEMORY "1008 out of memory"
def ES_ADMIN "1009 QM or the macro must run as administrator"
def MINUTE 6.9444444444e-4 ;;used with DATE variables
#opt obsoletedecl 0

#opt hidedecl 0
