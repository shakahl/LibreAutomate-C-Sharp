function# str&to str&cc str&bcc [flags] ;;flags: 0 name, 1 address, 2 name <address>.

 Shows 'Address Book' dialog.
 Populates to, cc and bcc variables with selected email names and|or addresses.
 Returns number of selected recipients or 0.

ref ___mapi "MAPI"

int i sh nr
if(MapiProfile(sh)) ret

___mapi.MapiRecipDesc* mr
str& s
if(!___mapi.MAPIAddress(sh 0 0 3 0 0 0 0 0 &nr &mr))
	for(i 0 nr)
		sel(mr[i].ulRecipClass)
			case 1 &s=&to
			case 2 &s=&cc
			case 3 &s=&bcc
		if(s.len) s+"; "
		str sn(mr[i].lpszName) sa(mr[i].lpszAddress)
		sel(flags&3)
			case 0 s+iif(sn.len sn sa)
			case 1 s+sa
			case else if(sn.len) s.formata("%s <%s>" sn sa); else s+sa

	___mapi.MAPIFreeBuffer(mr)

___mapi.MAPILogoff sh 0 0 0
ret nr
