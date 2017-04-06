 /
function $address double&lat double&lng [$region]

 Converts address (like "1600 Amphitheatre Parkway, Mountain View, CA", or "MyCity")
 into geographic coordinates (like latitude 37.423021 and longitude -122.083739).
 Uses <link "http://code.google.com/apis/maps/documentation/geocoding/">Google Geocoding</link>.
 Requires Internet connection.
 Error if failed.
 There are usage limits. Click the above link to read more. For example, the function may fail if called too frequently, or if called more than 2500 times/day.

 address - address.
 lat, lng, variables that receive latitude and longitude.
 region - region code. More info: click the above link.

 EXAMPLE
 double lat lng
 GeocodeAddressToCoordinates "London" lat lng
 out F"lat={lat}[]lng={lng}"


str s u a
a=address; a.escape(9)
u=F"http://maps.google.com/maps/api/geocode/xml?address={a}&sensor=false&region={region}"
IntGetFile u s
 out s

IXml x=CreateXml
x.FromString(s)
IXmlNode loc=x.Path("GeocodeResponse/result/geometry/location")
lat=val(loc.Child("lat").Value 2)
lng=val(loc.Child("lng").Value 2)

err+
	if(s.len) end F"Failed.  Server response:[]{s}"
	end _error
