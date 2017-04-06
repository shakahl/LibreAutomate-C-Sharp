 /
function nCode wParam KBDLLHOOKSTRUCT*h

out "0x%X" h.flags
 out h.vkCode
 1
 out g_current_keyboard_id
 if(h.vkCode=VK_MEDIA_PLAY_PAUSE) ret 1

ret CallNextHookEx(bikhook nCode wParam h)
