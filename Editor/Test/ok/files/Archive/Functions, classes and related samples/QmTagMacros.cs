 /
function $tag $qmItems [flags] ;;flags: 1 untag

 Adds tag to multiple QM items (macros etc), or removes.

 tag - tag name. The tag can exist or not.
 qmItems - multiline list of QM item names or paths.

 REMARKS
 May fail if currently there are no tags. Error "no such table: xTags". Then at first add a tag in the Tags dialog.
 Does not update the Tags dialog if it is open.

 EXAMPLES
 str s=
  Macro2238
  Function273
  Dialog4567
 QmTagMacros "auto-tag" s
 
 QmTagMacros "auto-tag" "Function273" 1 ;;untag single macro


Sqlite& x=_qmfile.SqliteBegin
lpstr sql
if(flags&1) sql="DELETE FROM xTags WHERE guid=? AND tag=?"
else sql="INSERT INTO xTags(guid,tag) SELECT ?1,?2 WHERE NOT EXISTS(SELECT 1 FROM xTags WHERE guid=?1 AND tag=?2)"
SqliteStatement p.Prepare(x sql)
p.BindText(2 tag)
str s; GUID g
foreach s qmItems
	_qmfile.SqliteItemProp(s 0 g); err end F"QM item not found: {s}" 8; continue
	p.BindBlob(1 &g sizeof(g))
	p.Exec; p.Reset

err+ end F"failed: {_error.description}" 8
_qmfile.SqliteEnd
