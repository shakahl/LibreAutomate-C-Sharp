function~ $attribute [flags] ;;attribute: "href", "src", "type", "name", "value", "id", "alt", "title", "onclick", "classid", or other;  flags: 0 interpolated, 1 case sensitive, 2 as in HTML.

 Gets attribute value.
 Error if the element cannot have the attribute.

 REMARKS
 QM 2.3.3. Uses flags. Ignored in older versions (bug).
 QM 2.3.3. Supports "class" as attribute. Error in older versions. Does not use flags.


if(!el) end ERR_INIT

sel attribute 1
	case "class" ret el.className
	case else ret el.getAttribute(attribute flags)

err+ end _error
