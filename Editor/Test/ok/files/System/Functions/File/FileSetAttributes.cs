 /
function $_file attr [flags] [DateTime&timeModified] [DateTime&timeCreated] [DateTime&timeAccessed] ;;flags: 0 set attr, 1 add attr, 2 remove attr, 3 don't change attr

 Changes standard file properties: attributes, times.
 Error if failed.

 _file - file path. Can be folder.
 attr - attributes.
 flags:
   0 - replace file attributes.
   1 - add file attribute flags specified in attr.
   2 - remove file attribute flags specified in attr.
   3 - don't change file attributes. Use when want to change only times.
 timeModified, timeCreated, timeAccessed - variables that contain file time, UTC. If omitted or 0, will not change.

 See also: <FileGetAttributes>.
 Added in: QM 2.4.1. Replaces SetAttr and SetFileTimes.

 EXAMPLES
  create a test-file and make it read-only and hidden
 str sf="$temp$\test_attr.txt"
 _s="test"; _s.setfile(sf)
 FileSetAttributes sf FILE_ATTRIBUTE_READONLY|FILE_ATTRIBUTE_HIDDEN 1
 out "0x%X" FileGetAttributes(sf)
 run sf "" "properties" "" 0x70000 ;;open the file Properties dialog
 1; del- sf

  set last-access time to time-now
 str sf="$my qm$\test\test.txt"
 DateTime t.FromComputerTime(1)
 FileSetAttributes sf 0 3 0 0 t
 run sf "" "properties" "" 0x70000


if flags&3<3
	word* w=@_s.expandpath(_file)
	int _a=GetFileAttributesW(w); if(_a=-1) goto ge
	sel(flags&3) case 1 attr|_a; case 2 attr=_a~attr
	if(attr!_a) if(!SetFileAttributesW(w attr)) goto ge

if &timeModified or &timeCreated or &timeAccessed
	__HFile f.Create(_file OPEN_EXISTING FILE_WRITE_ATTRIBUTES FILE_SHARE_READ|FILE_SHARE_WRITE); err goto ge
	if(!SetFileTime(f +&timeCreated +&timeAccessed +&timeModified)) goto ge

ret
 ge
end F"{ERR_FAILED} to set file attributes" 16
