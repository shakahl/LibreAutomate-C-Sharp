0.1
9
wait i
wait (j)
wait 0 WA "QM Help"
wait 9 -WA "???"
wait 0 -CU IDC_SIZEALL
wait 8 -P
wait 0 H 67
int+ v8=1
wait 0 -V v8
spe (7 - 5)
wait 0 -C 0xFFFFFF 111 12 id(1006 "QM Help")
wait aa(hu - 1) K (VK_TAB)
int u=wait(0 MR)
int a=wait(0 -WA "+_macr_dreamweaver_frame_window_")
int p=wait(0 -WC "QM Help")
wait 0 -WV "???"
wait 0 -WE "???"
