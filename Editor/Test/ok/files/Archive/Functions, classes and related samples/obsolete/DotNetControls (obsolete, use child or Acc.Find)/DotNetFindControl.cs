 /
function# hwndParent $controlName [$text] [$classname] [flags]

 Finds control in .NET (Windows Forms) window.
 Returns its handle. If does not find, returns 0.
 Fails if the process belongs to another user.
 On Vista, fails if QM is running as User and the process has higher integrity level.

 controlName - .NET control name (eg "Button1").
 other arguments - the same as with function child().

 See also: <DotNetShowControlNames>  <DotNetControls help>

 EXAMPLE
 int w1=win("Form1")
 int w2=DotNetFindControl(w1 "TextBox1")


#compile __DotNetControls
DotNetControls c.Init(hwndParent)
ret c.FindControl(controlName text classname flags)

err+
