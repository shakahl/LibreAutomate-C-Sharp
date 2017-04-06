type CHANGEVOLPROC = #
 ;;function# $ArcName Mode
def ERAR_BAD_ARCHIVE 13
def ERAR_BAD_DATA 12
def ERAR_ECLOSE 17
def ERAR_ECREATE 16
def ERAR_END_ARCHIVE 10
def ERAR_EOPEN 15
def ERAR_EREAD 18
def ERAR_EWRITE 19
def ERAR_MISSING_PASSWORD 22
def ERAR_NO_MEMORY 11
def ERAR_SMALL_BUF 20
def ERAR_UNKNOWN 21
def ERAR_UNKNOWN_FORMAT 14
type PROCESSDATAPROC = #
 ;;function# $Addr Size
dll- unrar #RARCloseArchive hArcData
dll- unrar #RARGetDllVersion
type RARHeaderData !ArcName[260] !FileName[260] Flags PackSize UnpSize HostOS FileCRC FileTime UnpVer Method FileAttr $CmtBuf CmtBufSize CmtSize CmtState
type RARHeaderDataEx !ArcName[1024] @ArcNameW[1024] !FileName[1024] @FileNameW[1024] Flags PackSize PackSizeHigh UnpSize UnpSizeHigh HostOS FileCRC FileTime UnpVer Method FileAttr $CmtBuf CmtBufSize CmtSize CmtState Reserved[1024]
dll- unrar #RAROpenArchive RAROpenArchiveData*ArchiveData
type RAROpenArchiveData $ArcName OpenMode OpenResult $CmtBuf CmtBufSize CmtSize CmtState
type RAROpenArchiveDataEx $ArcName @*ArcNameW OpenMode OpenResult $CmtBuf CmtBufSize CmtSize CmtState Flags Reserved[32]
dll- unrar #RAROpenArchiveEx RAROpenArchiveDataEx*ArchiveData
dll- unrar #RARProcessFile hArcData Operation $DestPath $DestName
dll- unrar #RARProcessFileW hArcData Operation @*DestPath @*DestName
dll- unrar #RARReadHeader hArcData RARHeaderData*HeaderData
dll- unrar #RARReadHeaderEx hArcData RARHeaderDataEx*HeaderData
dll- unrar RARSetCallback hArcData Callback UserData
 ;;Callback: function# msg UserData P1 P2
dll- unrar RARSetChangeVolProc hArcData ChangeVolProc
 ;;ChangeVolProc: function# $ArcName Mode
dll- unrar RARSetPassword hArcData $Password
dll- unrar RARSetProcessDataProc hArcData ProcessDataProc
 ;;ProcessDataProc: function# $Addr Size
def RAR_DLL_VERSION 4
def RAR_EXTRACT 2
def RAR_OM_EXTRACT 1
def RAR_OM_LIST 0
def RAR_OM_LIST_INCSPLIT 2
def RAR_SKIP 0
def RAR_TEST 1
def RAR_VOL_ASK 0
def RAR_VOL_NOTIFY 1
def UCM_CHANGEVOLUME 0
def UCM_NEEDPASSWORD 2
def UCM_PROCESSDATA 1
type UNRARCALLBACK = #
 ;;function# msg UserData P1 P2
type UNRARCALLBACK_MESSAGES = #
def _UNRAR_DLL_
