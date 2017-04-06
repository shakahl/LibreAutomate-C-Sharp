 out GetCurDir
 SetCurDir "$my qm$" 1
 out GetCurDir

 SaveMultiWinPos "ᶚᶚᶚᶚ" "Notepad[]WordPad"
 10
 RestoreMultiWinPos("ᶚᶚᶚᶚ")


 int w=win("Options" "#32770")
 Acc a.Find(w "COMBOBOX" "" "class=ComboBox[]id=1577[]value=Medium" 0x1004)
 a.CbSelect("Bigger")

 int w=win("Options" "#32770")
 Acc a.Find(w "PROPERTYPAGE" "" "class=#32770" 0x1004)
 ARRAY(Acc) b; int i
 a.GetChildObjects(b)
 for i 0 b.len
	 out b[i].Name

