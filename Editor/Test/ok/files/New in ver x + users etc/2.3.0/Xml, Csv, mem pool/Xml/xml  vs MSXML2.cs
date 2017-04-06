/exe
out
 2

lpstr sx=
 <?xml version="1.0" encoding="utf-8" standalone="yes" ?>
 <styling>
 	<entities>&lt;&gt;&amp;"'</entities>
 	<full a="aaa" b="bbb" c="ccc">text</full>
 	<common>
 		<a>0x808080</a>
 		<b>left<innertag>center</innertag>right</b>
 		<c>c1</c>
 		<c>c2</c>
 		<c>c3</c>
 		<c>c4</c>
 	</common>
 	<default>
 		<font>Courier New</font>
 	</default>
 	<empty />
 	text after empty
 	<notext />
 	<empty-attr a="1" />
 	<notext-attr b="" />
 	<lang name="qm">
 		<styles>
 			<s32 f="Courier' &quot;New" fs="8">Default</s32>
 			<s1 u="1">Tabs</s1>
 			<?pi1 abc ?>
 		</styles>
 		<misc>
 			<text>
 			line1
 			line2
 			</text>
 			<mixed-content>
 			line1
 			<?pi1 intext ?>
 			line2
 			</mixed-content>
 		</misc>
 	</lang>
 	<!--
 	<lang name="cpp">
 		<styles>
 		...
 		</styles>
 		<filetypes>*.cpp;*.h;...</filetypes>
 		<keywords>...</keywords>
 	</lang>
 	<lang name="vb" use_properties_file="1" />
 	-->
 	<![CDATA[some code]]>
 </styling>
 <?pi2 abc ?>
 <!--<tag></tag>-->

 out zx
 2

 ----------------------------------------------------------
Q &q
IXml x=CreateXml
Q &qq
x.FromString(sx)
 err out x.GetXmlParsingError; ret
Q &qqq
_s=x.RootElement.FirstChild.Name
Q &qqqq
outq
out x.Count

 ----------------------------------------------------------
 typelib MSXML2 {F5078F18-C551-11D3-89B9-0000F81FE221} 3.0
  typelib MSXML6 {F5078F18-C551-11D3-89B9-0000F81FE221} 6.0
 Q &q
 MSXML2.DOMDocument doc._create
  MSXML6.DOMDocument doc._create
 Q &qq
 doc.loadXML(sx)
 Q &qqq
 _s=doc.documentElement.firstChild.nodeName
  out doc.documentElement.firstChild
 Q &qqqq
 outq

 ----------------------------------------------------------
out _s
 5
 ret

 BEGIN PROJECT
 main_function  xml  vs MSXML
 exe_file  $my qm$\xml  vs MSXML.exe
 icon  $qm$\macro.ico
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  6
 end_hotkey  0
 guid  {A436CE25-75D7-4C63-884F-898FDF0C5598}
 END PROJECT
