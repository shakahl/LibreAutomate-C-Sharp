function $range EXCELFORMAT&x ;;style: use "Normal" to reset.  colors: 0xBBGGRR or index|0x80000000.  alignHor: 1 left, 2 right, 3 center, 7 default.  alignVert:  1 top, 2 bottom, 3 center, 7 default.  indentLevel: 1-15, -1 reset.  borders (flags): 1 top, 2 bottom, 4 left, 8 right, 16 inside hor, 32 inside vert, 64 diag down, 128 diag up, 0x100 remove, 0x200 remove diagonal.  borderThickness: 1-4.

 Sets cell format.

 range - cell, row, column or other range. <help>Excel range strings</help>.
 x - variable with formats:
   style - style name (string).
     In Excel there are several predefined styles ("Currency", "Percent"...). User can define more styles.
     Use "Normal" to reset all existing formatting.
   numberFormat - number format (string).
     To see number format strings for predefined formats: In Excel "Format Cells" dialog select a format and then select "Custom".
   fontName.
   fontBold, fontItalic, fontUnderline - font styles. Can be 1 (set) or 0 (reset if fontNormal 1).
   fontNormal - if 1, font not bold/italic/underline.
   fontSize.
   fontColor.
   cellColor.
   alignHor, alignVert - horizontal and vertical text alignment.
   textWrap, textMerge - text control. Can be 1 (set) or 0 (reset if textNormal 1).
   textNormal - if 1, text not wrap/merge.
   indentLevel - text indent level. Max 15. Use -1 to set 0.
   borders - which borders to modify (apply borderThickness, borderStyle, borderColor).
     One or more of flags: 1 top, 2 bottom, 4 left, 8 right, 16 inside hor, 32 inside vert, 64 diag down, 128 diag up, 0x100 remove, 0x200 remove diagonal.
     Examples: 0xf - all borders around, 0x30 - inside grid, 0x3ff - both.
     Tip: To set different borders for a range, call this function several times for the range. Use other variable or change/clear members of same variable.
   borderStyle - one of Excel constants: Excel.xlContinuous (1, default), xlDash, xlDashDot, xlDashDotDot xlDot, xlDouble, xlSlantDashDot, xlLineStyleNone.
   borderThickness - 1 dots, 2 thin (1-pixel), 3 medium, 4 thick.
   borderColor.

 REMARKS
 Before calling this function, declare an EXCELFORMAT variable and set its members for formats that you want to change.
 The function does not change formats for members that are 0 or "".
 The function sets formats in the order listed above.
 The function does not modify the variable.
 Colors can be specified in 2 ways:
   Color value in <help #IDP_COLOR>0xBBGGRR format</help>. Can be with flag 0x1000000, to make possible to use color 0.
   If with flag 0x80000000 - <google>Excel color index</google>. Index 0 is "Automatic".

 Added in: QM 2.3.3.
 Errors: Excel errors

 EXAMPLE
  /exe 1
 ExcelSheet es.Init
 EXCELFORMAT x
 x.style="Normal"
 x.fontBold=1
 x.fontColor=0x008000
 x.cellColor=24|0x80000000
 __ExcelState _es.Init(es 2) ;;temporarily disable Excel screen updating. Faster and less flickering.
 es.Format("B2:C8" x)
  ...
 _es.UpdatingRestore


WS

Excel.Range r=__Range(range)

if(x.style.len) r.Style=x.style

if(x.numberFormat.len) r.NumberFormat=x.numberFormat

if x.__fontStyle or x.fontSize or x.fontColor or x.fontName.len
	Excel.Font f=r.Font
	
	if(x.fontName.len) f.Name=x.fontName
	
	if(x.fontBold) f.Bold=-1; else if(x.fontNormal) f.Bold=0
	if(x.fontItalic) f.Italic=-1; else if(x.fontNormal) f.Italic=0
	if(x.fontUnderline) f.Underline=Excel.xlUnderlineStyleSingle; else if(x.fontNormal) f.Underline=Excel.xlUnderlineStyleNone
	
	if(x.fontSize) f.Size=x.fontSize
	
	if(x.fontColor&0x80000000) f.ColorIndex=x.fontColor&~0x80000000 ;;0 sets auto
	else if(x.fontColor) f.Color=x.fontColor&0xffffff

if x.cellColor
	Excel.Interior ir=r.Interior
	if(x.cellColor&0x80000000) ir.ColorIndex=x.cellColor&~0x80000000
	else ir.Color=x.cellColor&0xffffff

int h v
sel x.alignHor
	case 1 h=Excel.xlLeft
	case 2 h=Excel.xlRight
	case 3 h=Excel.xlCenter
	case 7 h=Excel.xlGeneral
if(h) r.HorizontalAlignment=h
sel x.alignVert
	case 1 v=Excel.xlTop
	case 2 v=Excel.xlBottom
	case 3 v=Excel.xlCenter
	case 7 v=Excel.xlGeneral
if(v) r.VerticalAlignment=v

if(x.textWrap) r.WrapText=-1; else if(x.textNormal) r.WrapText=0
if(x.textMerge) r.MergeCells=-1; else if(x.textNormal) r.MergeCells=0

if(x.indentLevel>0) r.IndentLevel=x.indentLevel
else if(x.indentLevel=-1) r.IndentLevel=0

if x.borders
	Excel.Borders bc=r.Borders
	if(x.borders&0x100) bc.LineStyle=Excel.xlLineStyleNone
	if(x.borders&0x200) bc.Item(5).LineStyle=Excel.xlLineStyleNone; bc.Item(6).LineStyle=Excel.xlLineStyleNone ;;the above does not remove diagonals
	
	int i bw mask=x.borders&0xFF
	if mask
		lpstr bt="[8][9][7][10][12][11][5][6]"
		sel(x.borderThickness) case 1 bw=Excel.xlHairline; case 2 bw=Excel.xlThin; case 3 bw=Excel.xlMedium; case 4 bw=Excel.xlThick
		
		for i 0 8
			Excel.Border b=bc.Item(bt[i])
			if mask&(1<<i)
				if(x.borderStyle) b.LineStyle=x.borderStyle; else if(!bw and b.LineStyle=Excel.xlLineStyleNone) b.LineStyle=Excel.xlContinuous
				if(bw) b.Weight=bw
				if x.borderColor
					if(x.borderColor&0x80000000) b.ColorIndex=x.borderColor&~0x80000000
					else b.Color=x.borderColor&0xffffff
	 tested: BorderAround cannot be used

err+ E
