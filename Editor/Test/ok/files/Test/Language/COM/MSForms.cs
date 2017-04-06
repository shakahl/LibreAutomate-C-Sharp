typelib MSForms {AC2DE821-36A2-11CF-8053-00AA006009FA} 2.0

opt waitmsg 1
 #opt dispatch 1
 MSForms.
MSForms.UserForm f._create
 f.DesignMode=0
 f._SetGridX(1)
f._SetHeight(100)
 MSForms.CheckBox b=+f.Controls._AddByClass(+uuidof(CheckBox))
MSForms.CheckBox b=+f.Controls.Add("Forms.CheckBox.1" "name" -1)
out b
b._SetHeight(100)
 int w
 f._GetWidth(&w)
 out w
 f._SetHeight(100)
 f._SetWidth(100)
 f.Width=10.0
 f.Height=0.0
 out f.Width
 f.
f.Visible=1
1
 f.Repaint
 5000
