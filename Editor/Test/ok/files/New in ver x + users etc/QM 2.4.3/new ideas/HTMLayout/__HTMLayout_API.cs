#opt err 1
def ACTIVATE_CHILD 146
type ADVANCE_TO = #
def ALT_KEY_PRESSED 0x4
def ANIMATION 0xA0
def BACK_IMAGE_AREA 0x40
def BEHAVIOR_ATTACH 1
def BEHAVIOR_DETACH 0
type BEHAVIOR_EVENTS = #
type BEHAVIOR_EVENT_PARAMS cmd heTarget he reason JSON_VALUE'data
type BEHAVIOR_METHOD_IDENTIFIERS = #
def BLOCK_BLOCK_ELEMENT 4
def BLOCK_TEXT_ELEMENT 3
def BORDER_BOX 0x20
def BUBBLING 0
def BUTTON_CLICK 0
def BUTTON_PRESS 1
def BUTTON_STATE_CHANGED 2
def BY_CODE 0
def BY_DEL_CHAR 5
def BY_DEL_CHARS 6
def BY_INS_CHAR 3
def BY_INS_CHARS 4
def BY_KEY_CLICK 1
def BY_KEY_NEXT 2
def BY_KEY_PREV 3
def BY_MOUSE 1
def BY_MOUSE_CLICK 0
def CLOSE_POPUP 158
def CONTAINER_RELATIVE 0x03
def CONTENT_BOX 0x00
def CONTEXT_MENU_REQUEST 0x10
def CONTEXT_MENU_SETUP 0xF
def CONTROL_KEY_PRESSED 0x1
def CTL_BUTTON 4
def CTL_CALENDAR 22
def CTL_CHECKBOX 5
def CTL_CURRENCY 16
def CTL_DATE 23
def CTL_DD_SELECT 9
def CTL_DECIMAL 15
def CTL_EDIT 2
def CTL_FORM 35
def CTL_FRAME 25
def CTL_FRAMESET 26
def CTL_GRAPHICS 27
def CTL_HIDDEN 32
def CTL_HTMLAREA 11
def CTL_HYPERLINK 18
def CTL_LIST 29
def CTL_MENU 20
def CTL_MENUBAR 19
def CTL_MENUBUTTON 21
def CTL_NO 0
def CTL_NUMERIC 3
def CTL_PASSWORD 12
def CTL_PROGRESS 13
def CTL_RADIO 6
def CTL_RICHTEXT 30
def CTL_SCROLLBAR 17
def CTL_SELECT_MULTIPLE 8
def CTL_SELECT_SINGLE 7
def CTL_SLIDER 14
def CTL_SPRITE 28
def CTL_TEXTAREA 10
def CTL_TIME 24
def CTL_TOOLBAR 34
def CTL_TOOLTIP 31
type CTL_TYPE = #
def CTL_UNKNOWN 1
def CTL_URL 33
def CURSOR_APPSTARTING 11
def CURSOR_ARROW 0
def CURSOR_CROSS 3
def CURSOR_DRAG_COPY 15
def CURSOR_DRAG_MOVE 14
def CURSOR_HAND 13
def CURSOR_HELP 12
def CURSOR_IBEAM 1
def CURSOR_NO 10
def CURSOR_SIZEALL 9
def CURSOR_SIZENESW 6
def CURSOR_SIZENS 8
def CURSOR_SIZENWSE 5
def CURSOR_SIZEWE 7
type CURSOR_TYPE = #
def CURSOR_UPARROW 4
def CURSOR_WAIT 2
def CVT_JSON_LITERAL 1
def CVT_JSON_MAP 2
def CVT_SIMPLE 0
type DATA_ARRIVED_PARAMS !*initiator !*data dataSize dataType status @*uri
def DATA_ELEMENT 0
type DATA_ROWS_PARAMS totalRecords firstRecord firstRowIdx lastRowIdx
type DEBUG_OUTPUT_PROC = #
 ;;function !*param character
def DISABLED_STATUS_CHANGED 0x12
def DISABLE_INITIALIZATION 0x80000000
def DOCUMENT_COMPLETE 152
def DO_CLICK 0
def DO_SWITCH_TAB 146
def DRAGGING 0x100
def DRAGGING_COPY 2
def DRAGGING_MOVE 1
type DRAGGING_TYPE = #
def DRAG_ENTER 0xA
def DRAG_LEAVE 0xB
def DRAG_REQUEST 0xC
def DRAW_BACKGROUND 0
def DRAW_CONTENT 1
type DRAW_EVENTS = #
def DRAW_FOREGROUND 2
type DRAW_PARAMS cmd hdc RECT'area reserved
def DROP 9
def DT_HAS_DATE 0x01
def DT_HAS_SECONDS 0x04
def DT_HAS_TIME 0x02
def DT_UTC 0x10
type EDIT_CHANGED_REASON = #
def EDIT_VALUE_CHANGED 4
def EDIT_VALUE_CHANGING 3
type ELEMENT_AREAS = #
def ELEMENT_COLLAPSED 0x90
type ELEMENT_COMPARATOR = #
 ;;function# he1 he2 !*param
def ELEMENT_EXPANDED 145
type ELEMENT_MODEL = #
type ELEMENT_STATE_BITS = #
type EVENT_GROUPS = #
type EVENT_REASON = #
type EXCHANGE_COMMANDS = #
type EXCHANGE_DATA_TYPE = #
type EXCHANGE_EVENTS = #
type EXCHANGE_PARAMS cmd !*target POINT'pos POINT'pos_view data_types drag_cmd *fetch_data
def EXC_COPY 1
def EXC_LINK 4
def EXC_MOVE 2
def EXC_NONE 0
def EXF_FILE 0x10
def EXF_HTML 0x02
def EXF_HYPERLINK 0x04
def EXF_JSON 0x08
def EXF_TEXT 0x01
def EXF_UNDEFINED 0
type ElementEventProc = #
 ;;function# !*tag he evtg !*prms
type ExpandoRelease = #
 ;;function _HTMLayoutElementExpando*pexp he
type FETCH_EXCHANGE_DATA = #
 ;;function# EXCHANGE_PARAMS*params data_type !**ppDataStart *pDataLength
def FIRST_APPLICATION_EVENT_CODE 0x100
def FIRST_APPLICATION_METHOD_ID 0x100
type FLOAT_VALUE = ^
type FOCUS_ADVANCE_CMD = #
type FOCUS_CAUSE = #
def FOCUS_END 3
type FOCUS_EVENTS = #
def FOCUS_GOT 1
def FOCUS_HOME 2
def FOCUS_LOST 0
def FOCUS_NEXT 0
type FOCUS_PARAMS cmd !*target by_mouse_click cancel
def FOCUS_PREV 1
def FOCUS_REMOVE 0xFFFFFFFE
def FOCUS_RESTORE 0xFFFFFFFF
def FORE_IMAGE_AREA 0x50
def FORM_RESET 151
def FORM_SUBMIT 150
type GESTURE_CMD = #
def GESTURE_FLAGS_ALL 0xFFFF
def GESTURE_FLAG_PAN_HORIZONTAL 0x0008
def GESTURE_FLAG_PAN_VERTICAL 0x0004
def GESTURE_FLAG_PAN_WITH_GUTTER 0x4000
def GESTURE_FLAG_PAN_WITH_INERTIA 0x8000
def GESTURE_FLAG_ROTATE 0x0002
def GESTURE_FLAG_TAP1 0x0010
def GESTURE_FLAG_TAP2 0x0020
def GESTURE_FLAG_ZOOM 0x0001
def GESTURE_PAN 2
type GESTURE_PARAMS cmd !*target POINT'pos POINT'pos_view flags delta_time SIZE'delta_xy ^delta_v
def GESTURE_REQUEST 0
def GESTURE_ROTATE 3
type GESTURE_STATE = #
def GESTURE_STATE_BEGIN 1
def GESTURE_STATE_END 4
def GESTURE_STATE_INERTIA 2
def GESTURE_TAP1 4
def GESTURE_TAP2 5
type GESTURE_TYPE_FLAGS = #
def GESTURE_ZOOM 1
def GET_ASYNC 0
def GET_TEXT_VALUE 1
def GET_VALUE 0xFD
def GO_FIRST 0
def GO_LAST 1
def GO_NEXT 2
def GO_NEXT_CHAR 4
def GO_PREV 3
def GO_PREV_CHAR 5
def HANDLED 0x10000
def HANDLE_ALL 0xFFFF
def HANDLE_BEHAVIOR_EVENT 0x0100
def HANDLE_DATA_ARRIVED 0x080
def HANDLE_DRAW 0x0040
def HANDLE_EXCHANGE 0x1000
def HANDLE_FOCUS 0x0004
def HANDLE_GESTURE 0x2000
def HANDLE_INITIALIZATION 0x0000
def HANDLE_KEY 0x0002
def HANDLE_METHOD_CALL 0x0200
def HANDLE_MOUSE 0x0001
def HANDLE_SCROLL 0x0008
def HANDLE_SIZE 0x0020
def HANDLE_TIMER 0x0010
type HELEMENT = #
def HISTORY_DROP 154
def HISTORY_NEXT 156
def HISTORY_PRIOR 155
def HISTORY_PUSH 153
def HISTORY_STATE_CHANGED 157
def HLDOM_INVALID_HANDLE 2
def HLDOM_INVALID_HWND 1
def HLDOM_INVALID_PARAMETER 4
def HLDOM_OK 0
def HLDOM_OK_NOT_HANDLED 0xFFFFFFFF
def HLDOM_OPERATION_FAILED 5
def HLDOM_PASSIVE_HANDLE 3
type HLDOM_RESULT = #
def HLM_LAYOUT_ONLY 0
def HLM_SHOW_SELECTION 1
def HLN_ATTACH_BEHAVIOR 0x00000B07
def HLN_BEHAVIOR_CHANGED 0x00000B08
def HLN_COMMAND_CLICK 1
def HLN_CONTROL_CREATED 0x00000B02
def HLN_CREATE_CONTROL 0x00000B00
def HLN_DATA_LOADED 0x00000B03
def HLN_DESTROY_CONTROL 0x00000B06
def HLN_DIALOG_CLOSE_RQ 0x00000B09
def HLN_DIALOG_CREATED 0x00000B0F
def HLN_DOCUMENT_COMPLETE 0x00000B04
def HLN_DOCUMENT_LOADED 0x00000B0A
def HLN_HYPERLINK 2
def HLN_LOAD_DATA 0x00000B01
def HLN_REFRESH_AREA 0x00000B1F
def HLN_SET_CURSOR 0x00000B21
def HLN_SET_TIMER 0x00000B20
def HLN_UPDATE_UI 0x00000B05
def HLRT_DATA_CURSOR 3
def HLRT_DATA_HTML 0
def HLRT_DATA_IMAGE 1
def HLRT_DATA_SCRIPT 4
def HLRT_DATA_STYLE 2
type HLTRESULT = #
def HLT_FILE_NOT_FOUND 3
def HLT_INVALID_FORMAT 2
def HLT_INVALID_HANDLE 1
def HLT_INVALID_PARAMETER 4
def HLT_INVALID_STATE 5
def HLT_OK 0
def HLT_OK_DELAYED 6
type HPOSITION he pos
type HRANGE = #
def HTMLAYOUT_ANIMATION_THREAD 5
type HTMLAYOUT_CALLBACK_RES = #
 ;;function# @*resourceUri $resourceType !*imageData imageDataSize
type HTMLAYOUT_CALLBACK_RES_EX = #
 ;;function# @*resourceUri $resourceType !*imageData imageDataSize !*prm
def HTMLAYOUT_CONNECTION_TIMEOUT 2
type HTMLAYOUT_DATA_LOADER = #
 ;;function# @*uri dataType *pDataWriter
 ;;pDataWriter: function @*uri dataType !*data dataLength
type HTMLAYOUT_DATA_WRITER = #
 ;;function @*uri dataType !*data dataLength
def HTMLAYOUT_FONT_SMOOTHING 4
def HTMLAYOUT_HTTPS_ERROR 3
type HTMLAYOUT_NOTIFY = #
 ;;function# uMsg wParam lParam !*vParam
type HTMLAYOUT_OPTIONS = #
type HTMLAYOUT_SCROLL_FLAGS = #
def HTMLAYOUT_SMOOTH_SCROLL 1
def HTMLAYOUT_TRANSPARENT_WINDOW 6
type HTMLITE = #
type HTMLITE_CALLBACK = #
 ;;function# hLite NMHDR*hdr
dll- "$qm$\htmlayout.dll" #HTMLayoutAnimateElement he *pAnimator !*animatorParam
 ;;pAnimator: function# he step !*animatorParam
dll- "$qm$\htmlayout.dll" #HTMLayoutAppendMasterCSS !*utf8 numBytes
dll- "$qm$\htmlayout.dll" #HTMLayoutAttachEventHandler he *pep !*tag
 ;;pep: function# !*tag he evtg !*prms
dll- "$qm$\htmlayout.dll" #HTMLayoutAttachEventHandlerEx he *pep !*tag subscription
 ;;pep: function# !*tag he evtg !*prms
type HTMLayoutCSSRuleDef rule_type $file_url file_line_no @*selector
dll- "$qm$\htmlayout.dll" #HTMLayoutCallBehaviorMethod he _METHOD_PARAMS*params
dll- "$qm$\htmlayout.dll" [HTMLayoutClassNameA]$HTMLayoutClassName
dll- "$qm$\htmlayout.dll" @*HTMLayoutClassNameW
dll- "$qm$\htmlayout.dll" #HTMLayoutClearAttributes he
dll- "$qm$\htmlayout.dll" #HTMLayoutClipboardCopy hWndHTMLayout
dll- "$qm$\htmlayout.dll" #HTMLayoutCloneElement he !**phe
dll- "$qm$\htmlayout.dll" #HTMLayoutCombineURL he @*szUrlBuffer UrlBufferSize
dll- "$qm$\htmlayout.dll" #HTMLayoutCommitUpdates hWndHTMLayout
dll- "$qm$\htmlayout.dll" #HTMLayoutControlGetType he *pType
dll- "$qm$\htmlayout.dll" #HTMLayoutControlGetValue he VALUE*pVal
dll- "$qm$\htmlayout.dll" #HTMLayoutControlSetValue he VALUE*pVal
dll- "$qm$\htmlayout.dll" #HTMLayoutCreateElement $tagname @*textOrNull !**phe
dll- "$qm$\htmlayout.dll" #HTMLayoutDataReady hwnd @*uri !*data dataLength
dll- "$qm$\htmlayout.dll" #HTMLayoutDataReadyAsync hwnd @*uri !*data dataLength dataType
dll- "$qm$\htmlayout.dll" #HTMLayoutDeclareElementType $name elementModel
dll- "$qm$\htmlayout.dll" #HTMLayoutDeleteElement he
dll- "$qm$\htmlayout.dll" #HTMLayoutDetachElement he
dll- "$qm$\htmlayout.dll" #HTMLayoutDetachEventHandler he *pep !*tag
 ;;pep: function# !*tag he evtg !*prms
dll- "$qm$\htmlayout.dll" #HTMLayoutDialog hWndParent POINT'position alignment style styleEx *notificationCallback *eventsCallback !*callbackParam !*html htmlLength
 ;;notificationCallback: function# uMsg wParam lParam !*vParam ;;eventsCallback: function# !*tag he evtg !*prms
type HTMLayoutElementAnimator = #
 ;;function# he step !*animatorParam
type HTMLayoutElementCallback = #
 ;;function# he !*param
type HTMLayoutElementExpando *finalizer
 ;;type HTMLayoutElementExpando = _HTMLayoutElementExpando
dll- "$qm$\htmlayout.dll" #HTMLayoutElementGetExpando he _HTMLayoutElementExpando**ppExpando
dll- "$qm$\htmlayout.dll" #HTMLayoutElementSetExpando he _HTMLayoutElementExpando*pExpando
dll- "$qm$\htmlayout.dll" #HTMLayoutEnqueueMeasure he
dll- "$qm$\htmlayout.dll" #HTMLayoutEnumElementStyles he *callback !*callback_prm
 ;;callback: function HTMLayoutCSSRuleDef*pdef !*callback_prm
dll- "$qm$\htmlayout.dll" #HTMLayoutEnumResources hWndHTMLayout *cb
 ;;cb: function# @*resourceUri $resourceType !*imageData imageDataSize
dll- "$qm$\htmlayout.dll" #HTMLayoutEnumResourcesEx hWndHTMLayout *cb !*cbPrm
 ;;cb: function# @*resourceUri $resourceType !*imageData imageDataSize !*prm
dll- "$qm$\htmlayout.dll" #HTMLayoutEnumerate he *pcb !*p forward
 ;;pcb: function# !*p he pos postype @code
type HTMLayoutEnumerationCallback = #
 ;;function# !*p he pos postype @code
dll- "$qm$\htmlayout.dll" #HTMLayoutFindElement hwnd POINT'pt !**phe
dll- "$qm$\htmlayout.dll" #HTMLayoutGetAttributeByName he $name @**p_value
dll- "$qm$\htmlayout.dll" #HTMLayoutGetAttributeCount he *p_count
dll- "$qm$\htmlayout.dll" #HTMLayoutGetCharacterRect he pos RECT*outRect
dll- "$qm$\htmlayout.dll" #HTMLayoutGetChildrenCount he *count
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementByUID hwnd uid !**phe
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementHtml he !**utf8bytes outer
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementHtmlCB he outer *cb !*cb_param
 ;;cb: function !*utf8 utf8_length !*param
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementHwnd he *p_hwnd rootWindow
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementIndex he *p_index
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementInnerText he !**utf8bytes
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementInnerText16 he @**utf16words
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementInnerTextCB he *cb !*cb_param
 ;;cb: function @*text text_length !*param
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementIntrinsicHeight he forWidth *pHeight
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementIntrinsicWidths he *pMinWidth *pMaxWidth
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementLocation he RECT*p_location areas
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementState he *pstateBits
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementText he @*characters *length
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementType he $*p_type
dll- "$qm$\htmlayout.dll" #HTMLayoutGetElementUID he *puid
dll- "$qm$\htmlayout.dll" #HTMLayoutGetFocusElement hwnd !**phe
dll- "$qm$\htmlayout.dll" #HTMLayoutGetMinHeight hWndHTMLayout width
dll- "$qm$\htmlayout.dll" #HTMLayoutGetMinWidth hWndHTMLayout
dll- "$qm$\htmlayout.dll" #HTMLayoutGetNthAttribute he n $*p_name @**p_value
dll- "$qm$\htmlayout.dll" #HTMLayoutGetNthChild he n !**phe
dll- "$qm$\htmlayout.dll" #HTMLayoutGetParentElement he !**p_parent_he
dll- "$qm$\htmlayout.dll" #HTMLayoutGetRootElement hwnd !**phe
dll- "$qm$\htmlayout.dll" #HTMLayoutGetScrollInfo he POINT*scrollPos RECT*viewRect SIZE*contentSize
dll- "$qm$\htmlayout.dll" !*HTMLayoutGetSelectedHTML hWndHTMLayout *pSize
dll- "$qm$\htmlayout.dll" #HTMLayoutGetStyleAttribute he $name @**p_value
dll- "$qm$\htmlayout.dll" #HTMLayoutHidePopup he
dll- "$qm$\htmlayout.dll" #HTMLayoutHttpRequest he @*url dataType requestType REQUEST_PARAM*requestParams nParams
dll- "$qm$\htmlayout.dll" #HTMLayoutInsertElement he !*hparent index
dll- "$qm$\htmlayout.dll" #HTMLayoutIsElementEnabled he *pEnabled
dll- "$qm$\htmlayout.dll" #HTMLayoutIsElementVisible he *pVisible
dll- "$qm$\htmlayout.dll" #HTMLayoutLoadFile hWndHTMLayout @*filename
dll- "$qm$\htmlayout.dll" #HTMLayoutLoadHtml hWndHTMLayout !*html htmlSize
dll- "$qm$\htmlayout.dll" #HTMLayoutLoadHtmlEx hWndHTMLayout !*html htmlSize @*baseUrl
type HTMLayoutModes = #
dll- "$qm$\htmlayout.dll" #HTMLayoutMoveElement he xView yView
dll- "$qm$\htmlayout.dll" #HTMLayoutMoveElementEx he xView yView width height
dll- "$qm$\htmlayout.dll" #HTMLayoutParseValue @*text textLength mode VALUE*pVal
dll- "$qm$\htmlayout.dll" #HTMLayoutPostEvent he appEventCode heSource reason
dll- "$qm$\htmlayout.dll" #HTMLayoutProc hwnd msg wParam lParam
dll- "$qm$\htmlayout.dll" #HTMLayoutProcND hwnd msg wParam lParam *pbHandled
dll- "$qm$\htmlayout.dll" #HTMLayoutProcW hwnd msg wParam lParam
dll- "$qm$\htmlayout.dll" #HTMLayoutProcessUIEvent he evt !*eventCtlStruct *bOutProcessed
dll- "$qm$\htmlayout.dll" #HTMLayoutRangeAdvancePos range cmd *c HPOSITION*pPos
dll- "$qm$\htmlayout.dll" #HTMLayoutRangeCreate he *pRange outer
dll- "$qm$\htmlayout.dll" #HTMLayoutRangeFromPositions he HPOSITION*pStart HPOSITION*pEnd
dll- "$qm$\htmlayout.dll" #HTMLayoutRangeFromSelection he *pRange
dll- "$qm$\htmlayout.dll" #HTMLayoutRangeInsertHtml HPOSITION*pPos !*htmlUtf8Bytes numBytes
dll- "$qm$\htmlayout.dll" #HTMLayoutRangeIsEmpty range *pResult
dll- "$qm$\htmlayout.dll" #HTMLayoutRangeRelease range
dll- "$qm$\htmlayout.dll" #HTMLayoutRangeReplace range !*htmlUtf8Bytes numBytes
dll- "$qm$\htmlayout.dll" #HTMLayoutRangeToHtml range !**pHtmlUtf8Bytes *numBytes
dll- "$qm$\htmlayout.dll" #HTMLayoutRender hWndHTMLayout hBmp RECT'area
dll- "$qm$\htmlayout.dll" #HTMLayoutRequestElementData he @*url dataType !*initiator
type HTMLayoutResourceType = #
dll- "$qm$\htmlayout.dll" #HTMLayoutScrollToView he flags
dll- "$qm$\htmlayout.dll" #HTMLayoutSelectElements he $CSS_selectors *callback !*param
 ;;callback: function# he !*param
dll- "$qm$\htmlayout.dll" #HTMLayoutSelectElementsW he @*CSS_selectors *callback !*param
 ;;callback: function# he !*param
dll- "$qm$\htmlayout.dll" #HTMLayoutSelectParent he $selector depth !**heFound
dll- "$qm$\htmlayout.dll" #HTMLayoutSelectParentW he @*selector depth !**heFound
dll- "$qm$\htmlayout.dll" #HTMLayoutSelectionExist hWndHTMLayout
dll- "$qm$\htmlayout.dll" #HTMLayoutSendEvent he appEventCode heSource reason *handled
dll- "$qm$\htmlayout.dll" #HTMLayoutSetAttributeByName he $name @*value
dll- "$qm$\htmlayout.dll" #HTMLayoutSetCSS hWndHTMLayout !*utf8 numBytes @*baseUrl @*mediaType
dll- "$qm$\htmlayout.dll" HTMLayoutSetCallback hWndHTMLayout *cb !*cbParam
 ;;cb: function# uMsg wParam lParam !*vParam
dll- "$qm$\htmlayout.dll" #HTMLayoutSetCapture he
dll- "$qm$\htmlayout.dll" #HTMLayoutSetDataLoader *pDataLoader
 ;;pDataLoader: function# @*uri dataType *pDataWriter
dll- "$qm$\htmlayout.dll" #HTMLayoutSetElementHtml he !*html htmlLength where
dll- "$qm$\htmlayout.dll" #HTMLayoutSetElementInnerText he !*utf8bytes length
dll- "$qm$\htmlayout.dll" #HTMLayoutSetElementInnerText16 he @*utf16words length
dll- "$qm$\htmlayout.dll" #HTMLayoutSetElementState he stateBitsToSet stateBitsToClear updateView
dll- "$qm$\htmlayout.dll" #HTMLayoutSetEventRoot he !**phePrevRoot
dll- "$qm$\htmlayout.dll" #HTMLayoutSetHttpHeaders hWndHTMLayout $httpHeaders httpHeadersLength
dll- "$qm$\htmlayout.dll" #HTMLayoutSetMasterCSS !*utf8 numBytes
dll- "$qm$\htmlayout.dll" #HTMLayoutSetMediaType hWndHTMLayout @*mediaType
dll- ??? #HTMLayoutSetMediaVars hWndHTMLayout VALUE*mediaVars
dll- "$qm$\htmlayout.dll" HTMLayoutSetMode hWndHTMLayout HTMLayoutMode
dll- "$qm$\htmlayout.dll" #HTMLayoutSetOption hWndHTMLayout option value
dll- "$qm$\htmlayout.dll" #HTMLayoutSetScrollPos he POINT'scrollPos smooth
dll- "$qm$\htmlayout.dll" #HTMLayoutSetStyleAttribute he $name @*value
dll- "$qm$\htmlayout.dll" #HTMLayoutSetTimer he milliseconds
dll- "$qm$\htmlayout.dll" #HTMLayoutSetTimerEx he milliseconds timerId
dll- "$qm$\htmlayout.dll" HTMLayoutSetupDebugOutput !*param pfOutput
 ;;pfOutput: function !*param character
dll- "$qm$\htmlayout.dll" #HTMLayoutShowPopup hePopup heAnchor placement
dll- "$qm$\htmlayout.dll" #HTMLayoutShowPopupAt hePopup POINT'pos mode
dll- "$qm$\htmlayout.dll" #HTMLayoutSortElements he firstIndex lastIndex *cmpFunc !*cmpFuncParam
 ;;cmpFunc: function# he1 he2 !*param
type HTMLayoutStyleRuleCallback = #
 ;;function HTMLayoutCSSRuleDef*pdef !*callback_prm
dll- "$qm$\htmlayout.dll" #HTMLayoutSwapElements he1 he2
dll- "$qm$\htmlayout.dll" #HTMLayoutTrackPopupAt hePopup POINT'posRoot mode !**pheItem
dll- "$qm$\htmlayout.dll" #HTMLayoutTranslateMessage MSG*lpMsg
dll- "$qm$\htmlayout.dll" #HTMLayoutTraverseUIEvent evt !*eventCtlStruct *bOutProcessed
dll- "$qm$\htmlayout.dll" #HTMLayoutUpdateElement he remeasure
dll- "$qm$\htmlayout.dll" #HTMLayoutUpdateElementEx he flags
dll- "$qm$\htmlayout.dll" #HTMLayoutUpdateWindow hWndHTMLayout
dll- "$qm$\htmlayout.dll" #HTMLayoutUrlEscape @*text spaceToPlus $buffer bufferLength
dll- "$qm$\htmlayout.dll" #HTMLayoutUrlUnescape $url @*buffer bufferLength
dll- "$qm$\htmlayout.dll" #HTMLayoutVisitElements he $tagName $attributeName @*attributeValue *callback !*param depth
 ;;callback: function# he !*param
dll- "$qm$\htmlayout.dll" #HTMLayoutWindowAttachEventHandler hwndLayout *pep !*tag subscription
 ;;pep: function# !*tag he evtg !*prms
dll- "$qm$\htmlayout.dll" #HTMLayoutWindowDetachEventHandler hwndLayout *pep !*tag
 ;;pep: function# !*tag he evtg !*prms
type HTMLayoutWriterCallbackB = #
 ;;function !*utf8 utf8_length !*param
type HTMLayoutWriterCallbackW = #
 ;;function @*text text_length !*param
dll- "$qm$\htmlayout.dll" #HTMLayout_UnuseElement he
dll- "$qm$\htmlayout.dll" #HTMLayout_UseElement he
dll- "$qm$\htmlayout.dll" #HTMLiteAdvanceFocus hLite where *pRes
dll- "$qm$\htmlayout.dll" #HTMLiteAttachEventHandler hLite *pep !*tag subscription
 ;;pep: function# !*tag he evtg !*prms
dll- "$qm$\htmlayout.dll" #HTMLiteCreateInstance
dll- "$qm$\htmlayout.dll" #HTMLiteDestroyInstance hLite
dll- "$qm$\htmlayout.dll" #HTMLiteDetachEventHandler hLite *pep !*tag
 ;;pep: function# !*tag he evtg !*prms
dll- "$qm$\htmlayout.dll" #HTMLiteFindElement hLite x y !**phe
dll- "$qm$\htmlayout.dll" #HTMLiteGetDocumentMinHeight hLite *v
dll- "$qm$\htmlayout.dll" #HTMLiteGetDocumentMinWidth hLite *v
dll- "$qm$\htmlayout.dll" #HTMLiteGetElementByUID hLite uid !**phe
dll- "$qm$\htmlayout.dll" #HTMLiteGetElementHTMLITE he *pHTMLite reserved
dll- "$qm$\htmlayout.dll" #HTMLiteGetFocusElement hLite !**phe *pbViewFocusState
dll- "$qm$\htmlayout.dll" #HTMLiteGetNextFocusable hLite where !*currentElement !**ppElement *pEndReached
dll- "$qm$\htmlayout.dll" #HTMLiteGetRootElement hLite !**phe
dll- "$qm$\htmlayout.dll" #HTMLiteGetTag hLite !**tag
dll- "$qm$\htmlayout.dll" #HTMLiteLoadHtmlFromFile hLite @*path
dll- "$qm$\htmlayout.dll" #HTMLiteLoadHtmlFromMemory hLite @*baseURI !*dataptr datasize
dll- "$qm$\htmlayout.dll" #HTMLiteMeasure hLite viewWidth viewHeight
dll- "$qm$\htmlayout.dll" #HTMLiteRender hLite hdc x y sx sy
dll- "$qm$\htmlayout.dll" #HTMLiteRenderEx hLite hdc dst_x dst_y dst_sx dst_sy src_x src_y src_sx src_sy
dll- "$qm$\htmlayout.dll" #HTMLiteRenderOnBitmap hLite hbmp x y sx sy
dll- "$qm$\htmlayout.dll" #HTMLiteSetCallback hLite *cb
 ;;cb: function# hLite NMHDR*hdr
dll- "$qm$\htmlayout.dll" #HTMLiteSetDataReady hLite @*url !*data dataSize
dll- "$qm$\htmlayout.dll" #HTMLiteSetDataReadyAsync hLite @*uri !*data dataSize type
dll- "$qm$\htmlayout.dll" #HTMLiteSetMediaType hLite $mediatype
dll- "$qm$\htmlayout.dll" #HTMLiteSetTag hLite !*tag
dll- "$qm$\htmlayout.dll" #HTMLiteTraverseUIEvent hLite evt !*eventCtlStruct *bOutProcessed
dll- "$qm$\htmlayout.dll" #HTMLiteUpdateView hLite
type HTMPRINT = #
def HV_BAD_PARAMETER 1
def HV_INCOMPATIBLE_TYPE 2
def HV_OK 0
def HV_OK_TRUE 0xFFFFFFFF
def HWND_DISCARD_CREATION 1
def HWND_TRY_DEFAULT 0
def HYPERLINK_CLICK 0x80
type INITIALIZATION_EVENTS = #
type INITIALIZATION_PARAMS cmd
def INIT_DATA_VIEW 147
def INLINE_BLOCK_ELEMENT 2
def INLINE_TEXT_ELEMENT 1
def IS_EMPTY 0xFC
type KEYBOARD_STATES = #
def KEY_CHAR 2
def KEY_DOWN 0
type KEY_EVENTS = #
type KEY_PARAMS cmd !*target key_code alt_state
def KEY_UP 1
type KeyValueCallback = #
 ;;function# !*param VALUE*pkey VALUE*pval
def LOAD_DISCARD 1
def LOAD_OK 0
type LPCBYTE = !*
type LPCBYTES = !*
type LPELEMENT_EVENT_PROC = #*
 ;;function# !*tag he evtg !*prms
type LPHTMLAYOUT_NOTIFY = #*
 ;;function# uMsg wParam lParam !*vParam
type LPNMHL_ATTACH_BEHAVIOR = NMHL_ATTACH_BEHAVIOR*
type LPNMHL_BEHAVIOR_CHANGED = NMHL_BEHAVIOR_CHANGED*
type LPNMHL_CREATE_CONTROL = NMHL_CREATE_CONTROL*
type LPNMHL_DATA_LOADED = NMHL_DATA_LOADED*
type LPNMHL_DESTROY_CONTROL = NMHL_DESTROY_CONTROL*
type LPNMHL_DIALOG_CLOSE_RQ = NMHL_DIALOG_CLOSE_RQ*
type LPNMHL_LOAD_DATA = NMHL_LOAD_DATA*
type LPNMHL_REFRESH_AREA = NMHL_REFRESH_AREA*
type LPNMHL_SET_CURSOR = NMHL_SET_CURSOR*
type LPNMHL_SET_TIMER = NMHL_SET_TIMER*
def MAIN_MOUSE_BUTTON 0x01
def MARGIN_BOX 0x30
def MAX_URL_LENGTH 2048
def MEASURE_DEEP 0x0002
def MEASURE_INPLACE 0x0001
def MENU_ITEM_ACTIVE 0xA
def MENU_ITEM_CLICK 0xB
type METHOD_PARAMS methodID
 ;;type METHOD_PARAMS = _METHOD_PARAMS
def MIDDLE_MOUSE_BUTTON 0x04
type MOUSE_BUTTONS = #
def MOUSE_CLICK 0xFF
def MOUSE_DCLICK 5
def MOUSE_DOWN 4
def MOUSE_ENTER 0
type MOUSE_EVENTS = #
def MOUSE_IDLE 8
def MOUSE_LEAVE 1
def MOUSE_MOVE 2
type MOUSE_PARAMS cmd !*target POINT'pos POINT'pos_document button_state alt_state cursor_type is_on_icon !*dragging dragging_mode
def MOUSE_TICK 7
def MOUSE_UP 3
def MOUSE_WHEEL 6
type NMHL_ATTACH_BEHAVIOR NMHDR'hdr !*element $behaviorName *elementProc !*elementTag elementEvents
type NMHL_BEHAVIOR_CHANGED NMHDR'hdr !*element $oldNames $newNames
type NMHL_COMMAND_CLICK NMHDR'hdr @szElementID[2048] he
type NMHL_CREATE_CONTROL NMHDR'hdr helement inHwndParent outControlHwnd reserved1 reserved2
type NMHL_DATA_LOADED NMHDR'hdr @*uri !*data dataSize dataType status
type NMHL_DESTROY_CONTROL NMHDR'hdr helement inoutControlHwnd reserved1
type NMHL_DIALOG_CLOSE_RQ NMHDR'hdr outCancel
type NMHL_LOAD_DATA NMHDR'hdr @*uri !*outData outDataSize dataType !*principal !*initiator
type NMHL_REFRESH_ARE NMHDR'hdr RECT'area
type NMHL_REFRESH_AREA NMHDR'hdr RECT'area
type NMHL_SET_CURSOR NMHDR'hdr cursorId
type NMHL_SET_TIMER NMHDR'hdr timerId elapseTime
def NO_DRAGGING 0
type OutputStreamProc = #
 ;;function# !*params data
def PADDING_BOX 0x10
type PHASE_MASK = #
def POPUP_DISMISSED 9
def POPUP_DISMISSING 0x13
def POPUP_READY 8
def POPUP_REQUEST 7
def POST_ASYNC 1
def PROP_MOUSE_BUTTON 0x02
def REDRAW_NOW 0x8000
type REQUEST_PARAM @*name @*value
def REQUEST_TOOLTIP 159
type REQUEST_TYPE = #
def RESET_STYLE_DEEP 0x10
def RESET_STYLE_THIS 0x20
def ROOT_RELATIVE 0x01
def ROWS_DATA_REQUEST 148
def SCROLLABLE_AREA 0x60
def SCROLL_BAR_GET_VALUE 6
def SCROLL_BAR_SET_VALUE 7
def SCROLL_END 1
type SCROLL_EVENTS = #
def SCROLL_HOME 0
def SCROLL_PAGE_MINUS 5
def SCROLL_PAGE_PLUS 4
type SCROLL_PARAMS cmd !*target pos vertical
def SCROLL_POS 6
def SCROLL_SLIDER_RELEASED 7
def SCROLL_SMOOTH 0x10
def SCROLL_STEP_MINUS 3
def SCROLL_STEP_PLUS 2
def SCROLL_TO_TOP 0x01
def SELECT_SELECTION_CHANGED 5
def SELECT_STATE_CHANGED 6
def SELF_RELATIVE 0x02
def SET_TEXT_VALUE 2
def SET_VALUE 0xFE
def SHIFT_KEY_PRESSED 0x2
def SIH_APPEND_AFTER_LAST 2
def SIH_INSERT_AT_START 1
def SIH_REPLACE_CONTENT 0
def SINKING 0x08000
def SOH_INSERT_AFTER 5
def SOH_INSERT_BEFORE 4
def SOH_REPLACE 3
def STATE_ACTIVE 0x00000004
def STATE_ANCHOR 0x00004000
def STATE_ANIMATING 0x00001000
def STATE_BUSY 0x00080000
def STATE_CHECKED 0x00000040
def STATE_COLLAPSED 0x00000400
def STATE_COPYING 0x00800000
def STATE_CURRENT 0x00000020
def STATE_DISABLED 0x00000080
def STATE_DRAG_OVER 0x00100000
def STATE_DRAG_SOURCE 0x01000000
def STATE_DROP_MARKER 0x02000000
def STATE_DROP_TARGET 0x00200000
def STATE_EMPTY 0x00040000
def STATE_EXPANDED 0x00000200
def STATE_FOCUS 0x00000008
def STATE_FOCUSABLE 0x00002000
def STATE_HOVER 0x00000002
def STATE_INCOMPLETE 0x00000800
def STATE_IS_LTR 0x10000000
def STATE_IS_RTL 0x20000000
def STATE_LINK 0x00000001
def STATE_MOVING 0x00400000
def STATE_OWNS_POPUP 0x00010000
def STATE_POPUP 0x08000000
def STATE_PRESSED 0x04000000
def STATE_READONLY 0x00000100
def STATE_SYNTHETIC 0x00008000
def STATE_TABFOCUS 0x00020000
def STATE_VISITED 0x00000010
def SYNTHESIZED 2
def TABLE_HEADER_CLICK 129
def TABLE_ROW_CLICK 130
def TABLE_ROW_DBL_CLICK 131
def TEXT_EDIT_CHAR_POS_AT_XY 11
def TEXT_EDIT_GET_CARET_POSITION 8
def TEXT_EDIT_GET_SELECTION 3
def TEXT_EDIT_GET_SELECTION_HTML 10
def TEXT_EDIT_GET_SELECTION_TEXT 9
def TEXT_EDIT_REPLACE_SELECTION 5
def TEXT_EDIT_SET_SELECTION 4
type TIMER_PARAMS timerId
def T_ARRAY 9
def T_BOOL 2
def T_BYTES 12
def T_CURRENCY 7
def T_DATE 6
def T_DOM_OBJECT 14
def T_FLOAT 4
def T_FUNCTION 11
def T_INT 3
def T_LENGTH 8
def T_MAP 10
def T_NULL 1
def T_OBJECT 13
def T_STRING 5
def T_UNDEFINED 0
def UI_STATE_CHANGED 149
type UPDATE_ELEMENT_FLAGS = #
def UT_CM 9
def UT_COLOR 15
def UT_DIP 13
def UT_EM 1
def UT_EX 2
def UT_IN 8
def UT_MM 10
def UT_OBJECT_ARRAY 0
def UT_OBJECT_CLASS 2
def UT_OBJECT_ERROR 5
def UT_OBJECT_FUNCTION 4
def UT_OBJECT_NATIVE 3
def UT_OBJECT_OBJECT 1
def UT_PC 12
def UT_PR 3
def UT_PT 11
def UT_PX 7
def UT_SP 4
def UT_SYMBOL 0xFFFF
def UT_URL 16
type VALUE t u %d
type VALUE_RESULT = #
type VALUE_STRING_CVT_TYPE = #
type VALUE_TYPE = #
type VALUE_UNIT_TYPE = #
type VALUE_UNIT_TYPE_DATE = #
type VALUE_UNIT_TYPE_OBJECT = #
def VIEW_RELATIVE 0x04
def VISIUAL_STATUS_CHANGED 0x11
dll- "$qm$\htmlayout.dll" #ValueBinaryData VALUE*pval !**pBytes *pnBytes
dll- "$qm$\htmlayout.dll" #ValueBinaryDataSet VALUE*pval !*pBytes nBytes type units
dll- "$qm$\htmlayout.dll" #ValueClear VALUE*pval
dll- "$qm$\htmlayout.dll" #ValueCompare VALUE*pval1 VALUE*pval2
dll- "$qm$\htmlayout.dll" #ValueCopy VALUE*pdst VALUE*psrc
dll- "$qm$\htmlayout.dll" #ValueElementsCount VALUE*pval *pn
dll- "$qm$\htmlayout.dll" #ValueEnumElements VALUE*pval *penum !*param
 ;;penum: function# !*param VALUE*pkey VALUE*pval
dll- "$qm$\htmlayout.dll" #ValueFloatData VALUE*pval ^*pData
dll- "$qm$\htmlayout.dll" #ValueFloatDataSet VALUE*pval ^data type units
dll- "$qm$\htmlayout.dll" #ValueFromString VALUE*pval @*str strLength how
dll- "$qm$\htmlayout.dll" #ValueGetValueOfKey VALUE*pval VALUE*pkey VALUE*pretval
dll- "$qm$\htmlayout.dll" #ValueInit VALUE*pval
dll- "$qm$\htmlayout.dll" #ValueInt64Data VALUE*pval %*pData
dll- "$qm$\htmlayout.dll" #ValueInt64DataSet VALUE*pval %data type units
dll- "$qm$\htmlayout.dll" #ValueIntData VALUE*pval *pData
dll- "$qm$\htmlayout.dll" #ValueIntDataSet VALUE*pval data type units
dll- "$qm$\htmlayout.dll" #ValueInvoke VALUE*pval VALUE*pthis argc VALUE*argv VALUE*pretval @*url
dll- "$qm$\htmlayout.dll" #ValueIsolate VALUE*pdst
dll- "$qm$\htmlayout.dll" #ValueNthElementKey VALUE*pval n VALUE*pretval
dll- "$qm$\htmlayout.dll" #ValueNthElementValue VALUE*pval n VALUE*pretval
dll- "$qm$\htmlayout.dll" #ValueNthElementValueSet VALUE*pval n VALUE*pval_to_set
dll- "$qm$\htmlayout.dll" #ValueSetValueToKey VALUE*pval VALUE*pkey VALUE*pval_to_set
dll- "$qm$\htmlayout.dll" #ValueStringData VALUE*pval @**pChars *pNumChars
dll- "$qm$\htmlayout.dll" #ValueStringDataSet VALUE*pval @*chars numChars units
dll- "$qm$\htmlayout.dll" #ValueToString VALUE*pval how
dll- "$qm$\htmlayout.dll" #ValueType VALUE*pval *pType *pUnits
def WM_BEHAVIOR_NOTIFY 0x00000464
def WPARAM_DISCARD_BUILTIN_DD_SUPP0RT 0x8000
def X1_MOUSE_BUTTON 0x08
def X2_MOUSE_BUTTON 0x10
def XCALL 0xFF
def X_DRAG 2
def X_DRAG_ENTER 0
def X_DRAG_LEAVE 1
def X_DROP 3
type _HTMLayoutElementExpando *finalizer
type _METHOD_PARAMS methodID
def reserved1 5
def reserved2 6
def reserved3 14
type NMHL_HYPERLINK NMHDR'hdr action @szHREF[2048] @szTarget[2048] he
type JSON_VALUE t u %d
