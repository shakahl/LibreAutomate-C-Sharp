int w=win("Options" "#32770") ;;QM Options dialog
Acc a.Find(w "PROPERTYPAGE" "" "class=#32770" 0x1004)
ARRAY(Acc) b
a.GetChildObjects(b -1)
int i
for i 0 b.len
	out b[i].Name
