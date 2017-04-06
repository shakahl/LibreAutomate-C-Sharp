out
str sFile="$qm$\qm.exe"

#compile __trid

if(!TrID_LoadDefsPack(_s.expandpath("$qm$"))) end ES_FAILED ;;assume TrIDDefs.TRD is in qm folder
if(!TrID_SubmitFileA(_s.expandpath(sFile))) end ES_FAILED
if(!TrID_Analyze) end ES_FAILED
str buf.all(260 2 0) s
int i r=TrID_GetInfo(TRID_GET_RES_NUM,0,buf)
 out r
for i 1 r+1
	TrID_GetInfo(TRID_GET_RES_FILETYPE,i,buf); s=buf.lpstr
	out "File type: %s",s
	TrID_GetInfo(TRID_GET_RES_FILEEXT,i,buf); s=buf.lpstr
	out "File ext: .%s",s
	r=TrID_GetInfo(TRID_GET_RES_POINTS,i,buf)
	out "Matching points: %d[]",r
