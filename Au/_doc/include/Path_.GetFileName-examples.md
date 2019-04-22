Examples:

path | result
- | -
`@"C:\A\B\file.txt"` | `"file.txt"`
`"file.txt"` | `"file.txt"`
`"file"` | `"file"`
`@"C:\A\B"` | `"B"`
`@"C:\A\B\"` | `"B"`
`@"C:\A\/B\/"` | `"B"`
`@"C:\"` | `""`
`@"C:"` | `""`
`@"\\network\share"` | `"share"`
`@"C:\aa\file.txt:alt.stream"` | `"file.txt:alt.stream"`
`"http://a.b.c"` | `"a.b.c"`
`"::{A}\::{B}"` | `"::{B}"`
`""` | `""`
`null` | `null`

Examples when *withoutExtension* true:

path | result
- | -
`@"C:\A\B\file.txt"` | `"file"`
`"file.txt"` | `"file"`
`"file"` | `"file"`
`@"C:\A\B"` | `"B"`
`@"C:\A\B\"` | `"B"`
`@"C:\A\B.B\"` | `"B.B"`
`@"C:\aa\file.txt:alt.stream"` | `"file.txt:alt"`
`"http://a.b.c"` | `"a.b"`
