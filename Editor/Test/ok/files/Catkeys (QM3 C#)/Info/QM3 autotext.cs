#region begin_script
...
#endregion

var m=new AutoText();
m.IgnoreCase=true;
m["text1"] = t => T("replacement1");
m["text2"] = t => T("replacement2");
m.IgnoreCase=false;
m["text3"] = t => T("replacement3");

m.Run();
}

}
