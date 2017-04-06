out

IntGetFile "http://forecast.weather.gov/MapClick.php?lat=41.34083&lon=-89.09083&FcstType=digitalDWML" _s
 _s.getmacro("weather_xml")

IXml x=CreateXml
x.FromString(_s)

ARRAY(IXmlNode) a
x.Path("dwml/data/parameters/weather/weather-conditions" a)

int i
for i 0 a.len
	out "---------------"
	IXmlNode n=a[i].FirstChild
	rep
		if(!n) break
		out "weather-type=%s;  coverage=%s" n.AttributeValue("weather-type") n.AttributeValue("coverage")
		n=n.Next
