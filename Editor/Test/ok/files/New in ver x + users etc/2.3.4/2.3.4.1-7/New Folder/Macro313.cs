out
str folder="$desktop$\xml to files"
mkdir folder

str sXml=
 <snippetcollection>
  <maincat name="php">
      <subcat name="strings">
         <snippet>
            <title>explode-example1</title>
            <content>txttxttxt1</content>
         </snippet>
         <snippet>
            <title>explode-example2</title>
            <content>txttxttx2t</content>
         </snippet>
       </subcat>
      <subcat name="strings2">
         <snippet>
            <title>explode-example3</title>
            <content>txttxttxt3</content>
         </snippet>
       </subcat>
   </maincat>
 </snippetcollection>
IXml x=CreateXml
x.FromString(sXml)

 create folder for <maincat>
IXmlNode nmc=x.Path("snippetcollection/maincat")
str mainFolderName=nmc.AttributeValue("name")
mainFolderName.ReplaceInvalidFilenameCharacters("_")
mkdir F"{folder}\{mainFolderName}"

 get all <subcat>
ARRAY(IXmlNode) a1 a2
nmc.Path("subcat" a1)
int i j
for i 0 a1.len ;;for each <subcat>
	IXmlNode& r1=a1[i]
	str folderName=r1.AttributeValue("name")
	out "---- %s" folderName
	folderName.ReplaceInvalidFilenameCharacters("_")
	mkdir F"{folder}\{mainFolderName}\{folderName}"
	 get all <snippet>
	r1.Path("snippet" a2)
	for j 0 a2.len ;;for each <snippet>
		IXmlNode& r2=a2[j]
		str title content
		title=r2.Child("title").Value
		content=r2.Child("content").Value
		out "title='%s', content='%s'" title content
		title.ReplaceInvalidFilenameCharacters("_")
		str filePath=F"{folder}\{mainFolderName}\{folderName}\{title}.txt"
		content.setfile(filePath)

run folder
