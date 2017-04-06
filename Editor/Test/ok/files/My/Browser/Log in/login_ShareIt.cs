 out _s.timeformat
int hwnd=val(_command)

 Acc a.FindFF(hwnd "INPUT" "" "id=e5_frm_password[]name=PASSWORD" 0x1004 3)
 err ret

str u p
u="200134962"; p="[*2C63D14DE999F91C03A7FB0B4007B50507*]"
 sel list("Software publisher[]Affiliate")
	 case 1
	 case 2 u="200146401"; p="slapta1"
	 case else ret

AutoPassword u p 2|4 hwnd 5
err ret
key T
