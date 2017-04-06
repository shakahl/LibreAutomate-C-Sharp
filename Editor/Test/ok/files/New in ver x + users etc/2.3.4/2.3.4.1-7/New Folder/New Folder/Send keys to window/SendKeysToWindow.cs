 /Macro1837
function hwnd ARRAY(KEYEVENT)'a

 Sends keys to a window using WM_KEYDOWN, WM_CHAR and other messages.

 hwnd - window handle. Usually it is a child window. If 0, sends to the focused window.
 a - the <help>key</help> function. If used as function, it does not send keys, but instead returns array that is used by this function. See example.

 REMARKS
 The window can be inactive (not the foreground or focused window). However then not everything will work, eg will not work menus.
 If the window is hidden, minimized or not in screen, the function is slow and unreliable.
 Tip: To send just text, instead use SendTextToWindow. It's faster. Or use this function, and before add <code>opt keychar 1</code>; then it will send text like SendTextToWindow.
 Tip: Sends Enter for new lines in text. For example, these do the same: <code>SendKeysToWindow 0 key("Text" Y)</code> and <code>SendTextToWindow 0 "Text[]"</code>. Here Y is Enter, [] is newline.
 This function is faster than <code>key</code>.
 Does not support autodelay (spe). Usually it is not necessary. In some cases may need to add some delay in the macro.
 Does not support opt slowkeys and other options, except opt keychar, which is supported if <code>key</code> is used to create the array.

 EXAMPLE
 int c=id(15 "Notepad")
 SendKeysToWindow c key(CaX "Line1[]Line2[]") ;;Ctrl+A, Delete, text


if(!hwnd) hwnd=child; if(!hwnd) ret

 TODO: flag to send newline char instead of Enter.

int i wparam lparam m up packet prevPacket alt tid sync
str ks.all(256)

ifk(A) alt=1 ;;TODO: unpress instead. Also test other mods, capslock, numlock.

#compile "____SyncPostKey"
__SyncPostKey x
if(!x.Begin(hwnd)) end ERR_FAILED ;;attach thread input to be able to set keyboard state etc

for i 0 a.len
	KEYEVENT k=a[i]
	
	if(k.flags&0x80) ;;wait
		i+1
		opt waitmsg -1
		wait (a[i].wt/1000.0)
		continue
	
	up=k.flags&KEYEVENTF_KEYUP
	packet=k.flags&KEYEVENTF_UNICODE
	
	if packet
		if(up) continue
		m=WM_CHAR; wparam=k.sc; lparam=1
	else
		wparam=k.vk; lparam=k.sc<<16|1
		
		if(up) lparam|0xC0000000
		if(k.flags&KEYEVENTF_EXTENDEDKEY) lparam|0x01000000
		
		if(alt) m=iif(up WM_SYSKEYUP WM_SYSKEYDOWN); lparam|0x20000000
		else m=iif(up WM_KEYUP WM_KEYDOWN)
		
		sel k.vk
			case [16,17,18,91] ;;Shift, Ctrl, Alt, Win
			if(sync) x.Sync(sync)
			
			GetKeyboardState ks
			byte& z=ks[k.vk]; if(up) z~0x80; else z|0x80; z^1
			 TODO: test L R
			SetKeyboardState ks
			
			if(k.vk=VK_MENU) alt=!up
	
	if(sync and !up and wparam=13) x.Sync(sync)
	if(sync and packet!=prevPacket) x.Sync(sync)
	PostMessage hwnd m wparam lparam; sync+1
	if(!up and !packet) x.Sync(sync) ;;without this, wm_char are after wm_keyup. This slows down very much, but in some windows unreliable without. Eg VS, Firefox.
	 if(!up and !packet) x.SleepMP
	 if(!up and !packet) 0.1
	 if(!up and wparam=13) x.Sync(sync) ;;without this, wm_char of Enter are after wm_keyup, and in some windows, eg Firefox, it adds 2 newlines
	 TODO: test, maybe multiple key messages are received before their char messages
	prevPacket=packet
 TODO: KEYEVENTF_SCANCODE.

if(sync) x.Sync(sync)
