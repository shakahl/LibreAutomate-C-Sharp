out
str sFile="$appdata$\FileZilla\sitemanager.xml"
 _s.getfile(sFile); out _s

IXml x._create
x.FromFile(sFile)
ARRAY(IXmlNode) a
x.RootElement.GetAll(0 a)
int i
for i 0 a.len
	IXmlNode& n=a[i]
	sel n.Name
		case "Server"
		str sHost=n.Child("Host").Value
		out F"Server: {sHost}"
		
		case "Folder"
		str sFolder=a[i+1].Value; sFolder.trim ;;it is 'text' node after this 'element' node
		out F"Folder: {sFolder}"
		