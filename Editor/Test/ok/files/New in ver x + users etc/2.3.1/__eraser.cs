def ERASER_API
def ERASER_DATA_DRIVES 0
def ERASER_DATA_FILES 1
type ERASER_DATA_TYPE = #
def ERASER_ERROR 0xFFFFFFFF
def ERASER_ERROR_CONTEXT 0xFFFFFFF5
def ERASER_ERROR_DENIED 0xFFFFFFF1
def ERASER_ERROR_EXCEPTION 0xFFFFFFF6
def ERASER_ERROR_INIT 0xFFFFFFF4
def ERASER_ERROR_MEMORY 0xFFFFFFF8
def ERASER_ERROR_NOTIMPLEMENTED 0xFFFFFFE0
def ERASER_ERROR_NOTRUNNING 0xFFFFFFF2
def ERASER_ERROR_PARAM1 0xFFFFFFFE
def ERASER_ERROR_PARAM2 0xFFFFFFFD
def ERASER_ERROR_PARAM3 0xFFFFFFFC
def ERASER_ERROR_PARAM4 0xFFFFFFFB
def ERASER_ERROR_PARAM5 0xFFFFFFFA
def ERASER_ERROR_PARAM6 0xFFFFFFF9
def ERASER_ERROR_RUNNING 0xFFFFFFF3
def ERASER_ERROR_THREAD 0xFFFFFFF7
type ERASER_EXPORT = #
type ERASER_HANDLE = #
def ERASER_INVALID_CONTEXT 0xFFFFFFFF
type ERASER_METHOD = #
def ERASER_METHOD_DOD 2
def ERASER_METHOD_DOD_E 3
def ERASER_METHOD_FIRST_LAST_2KB 5
def ERASER_METHOD_GUTMANN 1
def ERASER_METHOD_LIBRARY 0
def ERASER_METHOD_PSEUDORANDOM 4
def ERASER_METHOD_SCHNEIER 6
def ERASER_OK 0
type ERASER_OPTIONS_PAGE = #
def ERASER_PAGE_DRIVE 0
def ERASER_PAGE_FILES 1
def ERASER_RANDOM_SLOW_POLL _T("EraserSlowPollEnabled")
def ERASER_REGISTRY_AUTHOR _T("Software\Heidi Computers Ltd")
def ERASER_REGISTRY_BASE _T("Software\Heidi Computers Ltd\Eraser\5.8")
def ERASER_REGISTRY_LIBRARY _T("Library")
def ERASER_REGISTRY_PROGRAM _T("Software\Heidi Computers Ltd\Eraser")
def ERASER_REGISTRY_RESULTS_FILES _T("ResultsForFiles")
def ERASER_REGISTRY_RESULTS_UNUSEDSPACE _T("ResultsForUnusedSpace")
def ERASER_REGISTRY_RESULTS_WHENFAILED _T("ResultsOnlyWhenFailed")
def ERASER_REMOVE_FOLDERONLY 0
def ERASER_REMOVE_RECURSIVELY 1
type ERASER_RESULT = #
def ERASER_TEST_PAUSED 3
def ERASER_URL_EMAIL _T("mailto:support@heidi.ie")
def ERASER_URL_HOMEPAGE _T("http://www.heidi.ie/eraser/")
def ERASER_WIPE_BEGIN 0
def ERASER_WIPE_DONE 2
def ERASER_WIPE_UPDATE 1
def ERASEXT_REGISTRY_ENABLED _T("ErasextEnabled")
def ERASEXT_REGISTRY_RESULTS _T("ResultsErasext")
def E_IN
def E_INOUT
type E_INT16 = @
type E_INT32 = #
type E_INT64 = %
type E_INT8 = !
def E_OUT
type E_PINT16 = @*
type E_PINT32 = #*
type E_PINT64 = %*
type E_PINT8 = !*
type E_PUINT16 = @*
type E_PUINT32 = #*
type E_PUINT64 = %*
type E_PUINT8 = !*
type E_UINT16 = @
type E_UINT32 = #
type E_UINT64 = %
type E_UINT8 = !
type EraserErrorHandler = #
 ;;function# $a b !*c !*d
dll C_macro ISNT_SUBFOLDER lpsz
 ;;((lpsz)[0]==_T('.')&&((lpsz)[1]==_T('\0')||((lpsz)[1]==_T('.')&&(lpsz)[2]==_T('\0'))))
dll C_macro IS_SUBFOLDER lpsz
 ;;(!((lpsz)[0]==_T('.')&&((lpsz)[1]==_T('\0')||((lpsz)[1]==_T('.')&&(lpsz)[2]==_T('\0')))))
def NODEFAULT __assume(0)
def WM_ERASERNOTIFY 0x0000040A
def _GUID_ERASER "Eraser.{D5BBB6C1-64F1-11d1-A87C-444553540000}"
dll C_macro bitSet x mask
 ;;(((x)&(mask))!=0)
dll ??? !convEraseMethod mIn
def diskClusterTips 0x00000040
def diskDirEntries 0x00000080
def diskFreeSpace 0x00000020
dll ??? #eraserAddItem a !*b @c
dll ??? #eraserClearItems a
dll ??? #eraserCompleted a !*b
dll ??? #eraserCreateContext *a
dll ??? #eraserCreateContextEx *a !b @c !d
dll ??? #eraserDestroyContext a
dll ??? #eraserDispFlags a !*b
def eraserDispInit 0x00000040
def eraserDispItem 0x00000020
def eraserDispMessage 0x00000004
def eraserDispPass 0x00000001
def eraserDispProgress 0x00000008
def eraserDispReserved 0x00000080
def eraserDispStop 0x00000010
def eraserDispTime 0x00000002
dll ??? #eraserEnd
dll C_macro eraserError x
 ;;(!((x)>=0))
dll ??? #eraserErrorString a @b !*c @*d
dll ??? #eraserErrorStringCount a @*b
dll ??? #eraserFailed a !*b
dll ??? #eraserFailedCount a *b
dll ??? #eraserFailedString a b !*c @*d
dll ??? #eraserGetClusterSize !*a @b *c
dll ??? #eraserGetDataType a *b
dll ??? #eraserGetFreeDiskSpace !*a @b %*c
dll ??? #eraserGetWindow a *b
dll ??? #eraserGetWindowMessage a *b
dll ??? #eraserInit
dll ??? #eraserIsRunning a !*b
dll ??? #eraserIsValidContext a
dll C_macro eraserIsValidDataType x
 ;;(((x)>=0)&&((x)<=1))
dll C_macro eraserIsValidMethod x
 ;;(((x)>=0)&&((x)<=6))
dll C_macro eraserOK x
 ;;((x)>=0)
dll ??? #eraserProgGetCurrentDataString a !*b @*c
dll ??? #eraserProgGetCurrentPass a @*b
dll ??? #eraserProgGetMessage a !*b @*c
dll ??? #eraserProgGetPasses a @*b
dll ??? #eraserProgGetPercent a !*b
dll ??? #eraserProgGetTimeLeft a *b
dll ??? #eraserProgGetTotalPercent a !*b
dll ??? #eraserRemoveFile !*a @b
dll ??? #eraserRemoveFolder !*a @b !c
dll ??? #eraserSetDataType a b
dll ??? #eraserSetErrorHandler a pfn !*fnParam
 ;;pfn: function# $a b !*c !*d
dll ??? #eraserSetFinishAction param1 action
dll ??? #eraserSetWindow a b
dll ??? #eraserSetWindowMessage a b
dll ??? #eraserShowOptions a b
dll ??? #eraserShowReport a b
dll ??? #eraserStart a
dll ??? #eraserStartSync a
dll ??? #eraserStatGetArea a %*b
dll ??? #eraserStatGetTime a *b
dll ??? #eraserStatGetTips a %*b
dll ??? #eraserStatGetWiped a %*b
dll ??? #eraserStop a
dll ??? #eraserTerminated a !*b
dll ??? #eraserTestContinueProcess a
dll ??? #eraserTestEnable a
def fileAlternateStreams 0x00000004
def fileClusterTips 0x00000001
def fileNames 0x00000002
dll C_macro setBit x mask
 ;;(x)|=(mask)
dll C_macro unsetBit x mask
 ;;(x)&=~(mask)
