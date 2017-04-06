int w=act(win("ControlsHelp - Sandcastle Help File Builder" "*.Window.*"))
SHFB_focus_editor
key Ca
str s.getsel
IXml x._create
x.FromString(s)
IXmlNode nr=x.Path("topic/developerConceptualDocument")
out nr
