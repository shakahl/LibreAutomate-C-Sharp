Examples:

path | result
- | -
`@"C:\A\B\file.txt"` | `@"C:\A\B"`
`"file.txt"` | `""`
`@"C:\A\B\"` | `@"C:\A"`
`@"C:\A\/B\/"` | `@"C:\A"`
`@"C:\"` | `null`
`@"\\network\share"` | `null`
`"http:"` | `null`
`@"C:\aa\file.txt:alt.stream"` | `"C:\aa"`
`"http://a.b.c"` | `"http:"`
`"::{A}\::{B}"` | `"::{A}"`
`""` | `""`
`null` | `null`

Examples when *withSeparator* true:

path | result
- | -
`@"C:\A\B"` | `@"C:\A\"` (not `@"C:\A"`)
`"http://x.y"` | `"http://"` (not `"http:"`)
