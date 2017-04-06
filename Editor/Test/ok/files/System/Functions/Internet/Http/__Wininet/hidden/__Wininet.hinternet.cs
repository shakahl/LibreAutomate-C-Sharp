function# [flags] ;;flags: 1 root handle

 Returns HINTERNET handle that can be used with Windows Internet functions.
 By default it is second-level handle, opened by InternetConnect, which is called by Connect. It is 0 before Connect and after Disconnect.
 If flag 1, it is first-level handle opened by InternetOpen.

 Added in: QM 2.3.2.


if(flags&1)
	if(!m_hitop) Init
	ret m_hitop
ret m_hi
