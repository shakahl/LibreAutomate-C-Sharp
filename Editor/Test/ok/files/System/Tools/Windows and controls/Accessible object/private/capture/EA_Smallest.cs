 /
function# action [hDlg] [POINT'p] ;;action: 0 off, 1 on/off (toggle), 2 get obj, 3 get state (1 if on)

 Workaround for bugs in some apps (eg Silverlight in FF), where some objects are covered by their parent objects. Also for Java windows that don't provide object from x y.
 tested: a.a.hittest() also gets self.


___EA- dA
ARRAY(___EA_ARRAY)& ar=dA.ar
int i
RECT r

sel action
	case 0
	dA.isCaptureSmallest=0
	
	case 1
	if !dA.isCaptureSmallest
		if !dA.hwnd
			_s=
 Use this to capture objects that cannot be captured normally because there is other bigger transparent object on top.
 
 Capture at least 2 times in a window. At first capture normally. If it is incorrect object, check 'Capture smallest' and capture again.
 
 The second time QM uses different method to get object from mouse. It gets the smallest object. It uses this method only for objects that currently are in the object tree (invisible too).
			
			mes _s "" "i"
			ret
		
		if(ar.len<2) EA_Proc hDlg
		int hCur=SetCursor(LoadCursor(0 +IDC_WAIT))
		for(i 0 ar.len) EA_GetRect ar[i].a ar[i].r
		SetCursor hCur
		dA.isCaptureSmallest=1
	else
		dA.isCaptureSmallest=0
	
	case 2
	if(!dA.isCaptureSmallest) ret -1
	
	int iSm(-1) sizeSm size
	for i 0 ar.len
		r=ar[i].r
		if(!PtInRect(&r p.x p.y)) continue
		size=(r.right-r.left)*(r.bottom-r.top)
		if(size>0) if(sizeSm=0 or size<=sizeSm) sizeSm=size; iSm=i
	
	ret iSm
	
	case 3
	ret dA.isCaptureSmallest
