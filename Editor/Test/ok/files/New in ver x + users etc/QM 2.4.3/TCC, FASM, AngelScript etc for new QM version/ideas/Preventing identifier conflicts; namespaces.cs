QM language keywords and classes always beging with lowercase, eg abc or abcDef or _abc. Class functions too.
User classes must begin with uppercase, eg Abc or _Abc. Error if not.

All QM classes are in 'system' namespace. The namespace is implicitly used by all scripts.
If a script wants to use another namespace (except system and own), it must add: using Namespace.

NAMESPACES

Declaration:
namespace Name
... (code)

If a namespace not defined, the code belongs to an implicit 'global' namespace.

Nested namespaces:
namespace Nested{
...
}
namespace{ //nameless
...
}
