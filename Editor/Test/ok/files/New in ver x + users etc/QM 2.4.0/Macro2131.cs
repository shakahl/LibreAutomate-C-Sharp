 key F9
ARRAY(INPUT) a.create(2)
a[0].type=INPUT_KEYBOARD
a[0].ki.wVk=VK_F9
a[1]=a[0]
a[1].ki.dwFlags=KEYEVENTF_KEYUP
SendInput(a.len &a[0] sizeof(INPUT))
