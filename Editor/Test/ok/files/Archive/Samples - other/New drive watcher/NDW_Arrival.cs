 /NewDriveWatcher
function DEV_BROADCAST_HDR*hdr

if(hdr.dbch_devicetype!=DBT_DEVTYP_VOLUME) ret
DEV_BROADCAST_VOLUME* v=+hdr
if(v.dbcv_flags&DBTF_NET) ret
 out "0x%X 0x%X" v.dbcv_unitmask v.dbcv_flags

str sd=" :\"
int i m=v.dbcv_unitmask
for i 0 32
	if(m&1)
		sd[0]='A'+i
		mac "NewDriveInserted" "" sd
	m>>1
