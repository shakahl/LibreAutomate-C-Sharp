function ~name ~value [flags] ;;flags: 1 file, 2 free

 Adds form field to an internal array.
 <help>Http.PostFormData</help> uses the array if its argument 'a' is 0.

 name - field name. Same as "name" field in form's HTML.
 value - field value. Same as "value" field in form's HTML. If it is file field, must specify file.
 flags:
   1 - it is file field.
   2 - free the array. Name and value are ignored and can be "".

 Added in: QM 2.3.2.


if(flags&2) m_ap=0; ret

POSTFIELD& r=m_ap[]
r.name.swap(name)
r.value.swap(value)
r.isfile=flags&1
