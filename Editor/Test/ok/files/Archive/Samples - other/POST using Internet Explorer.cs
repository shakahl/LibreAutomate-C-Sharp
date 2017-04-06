out
str url="http://www.quickmacros.com/form2.php"
str post="a=1&b=2"

sel list("IntPost[]InternetExplorer.Navigate" _s.format("posts %s to %s" post url))
	case 1
	IntPost url post _s
	out _s
	
	case 2
	SHDocVw.InternetExplorer ie
	ie=web("" 8)
	 on Win XP and 200 also works: ie._create; ie.Visible=-1
	
	VARIANT v vh
	vh="Content-Type: application/x-www-form-urlencoded[]"
	
	ARRAY(byte) a.create(post.len)
	memcpy(&a[0] post a.len)
	v.attach(a)
	
	ie.Navigate(url @ @ v vh)
