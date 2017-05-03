#region begin_script
...
#endregion

void Function1()
{
//menu

var m=new CatMenu();

m["Item 1"] = Function2;
m["Item 2"] = delegate(CatMenu.ItemData t) { Out("test"); };
m["Item 3", 3] = t => Out("test"); //3 is id; also can be specified style, state, etc

m.Show();

//toolbar

var m=new CatBar();

m["Item 1"] = Function3;
m["Item 2"] = delegate(CatBar.ItemData t) { Out("test"); };
m["Item 3", 3] = t => Out("test"); //3 can be flags etc

m.ShowFree();
 or
m.ShowAttach(hwnd);

}

void Function2(CatMenu.ItemData t)
{
}

void Function3(CatBar.ItemData t)
{
}

}
