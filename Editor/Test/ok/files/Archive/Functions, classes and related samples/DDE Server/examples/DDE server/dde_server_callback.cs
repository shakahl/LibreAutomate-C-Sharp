 /DDE_server
function! action $topic $item str&data idInst hConv cbParam reserved

 Sample callback function that can be used with DdeServerStart.

 action - DDE action. One of XTYP_ constants, as used in the code below.
 topic - topic. On XTYP_DISCONNECT 0.
 item - item. Used only with XTYP_POKE and XTYP_REQUEST. With other actions 0.
 data - data. On XTYP_EXECUTE and XTYP_POKE it is the sent data or command string. On XTYP_REQUEST this function must store a string to return it to the client. Not used with other actions.
 idInst - DDE server's instance identifier. Can be used with DDE API functions.
 hConv - DDE conversation handle. On XTYP_CONNECT 0.
 cbParam - cbParam passed to DdeServerStart.
 reserved - currently not used.

 Return: 1 success, 0 failed.

 REMARKS
 This function will be called when clients connect, disconnect, ececute, poke or request.
 This function now just shows action and used arguments. To make it useful, edit it. For example, on XTYP_EXECUTE you can parse data and execute some code, for example run a macro.
 This function can use thread variables to store private data. The variables will not be shared between DDE servers, because a thread cannot have multiple DDE servers.


sel action
	case XTYP_CONNECT
	out "XTYP_CONNECT: topic='%s'" topic
	
	case XTYP_CONNECT_CONFIRM
	out "XTYP_CONNECT_CONFIRM: hConv=%i, topic='%s'" hConv topic
	
	case XTYP_DISCONNECT
	out "XTYP_DISCONNECT: hConv=%i" hConv
	
	case XTYP_EXECUTE
	out "XTYP_EXECUTE: hConv=%i, topic='%s', data='%s'" hConv topic data
	
	case XTYP_POKE
	out "XTYP_POKE: hConv=%i, topic='%s', item='%s', data='%s'" hConv topic item data
	
	case XTYP_REQUEST
	out "XTYP_REQUEST: hConv=%i, topic='%s', item='%s'" hConv topic item
	data="data"

ret 1
