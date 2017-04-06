Don't use such QM item types.
Instead create menu etc in macro:

#region menu
Menu x
x.Add("Label" {code} "icon")
x.Add("Label" {code} "icon")
int i=x.Show
#endregion

#region toolbar
Toolbar x
x.Add("Label" {code} "icon")
x.Add("Label" {code} "icon")
int htb=x.Show
#endregion

#region autotext
Autotext x
x.Add("Label" {code} "icon")
x.Add("Label" {code} "icon")
x.Register
#endregion
