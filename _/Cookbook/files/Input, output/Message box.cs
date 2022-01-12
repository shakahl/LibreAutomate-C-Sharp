//Doc comments (/// TEXT and /** TEXT */) are rendered as text.
//	Can contain <see> and Au output tags.
//	Must be at the start of a line.
//Other code is rendered as code. Including //comments.
//	If { CODE } or if(...){ CODE }, only CODE is used. In if-CODE removes 1 tab indentation.
//Code before the first doc comment is not rendered. It can be used for testing code examples.
//	For example can contain using directives or set a variable for if-CODE blocks (to select block(s) to run when testing the script).

int e = 2;

/// A quick brown <b>fox</b> jumped over != the lazy,,,, dog.
/// <see cref="Console"/> <see cref="System.Console"/> <see cref="Console.Write"/> <see cref="System.Console.Write(char)"/> <see cref="run.it"/> <see cref="run.console"/>
/// <link 'https://www.quickmacros.com'>link</link>
print.it("text");

/// A quick brown fox jumped over the lazy dog.
/// A quick brown fox jumped over the lazy dog.
{
print.it("text");
}
/// A quick <b>brown</b> fox jumped over the lazy dog.
if(e==2) {
	print.it(4);
	Console.Write(4);
}
/// A quick brown fox jumped over the lazy dog.

/// A quick brown fox jumped over the lazy dog.

//Local functions are code examples too (not only body).
void Local(int i) {
	Au.print.it(1);
}

//As well as types and namespaces.
class C1 {
	/// A quick brown fox jumped over the lazy dog.
	/// A quick brown fox jumped over the lazy dog.
	public int Prop => 0;
}

namespace Na {
	/// A quick brown fox jumped over the lazy dog.
	/// A quick brown fox jumped over the lazy dog.
	class C1 {
		/// A quick brown fox jumped over the lazy dog.
		/// A quick brown fox jumped over the lazy dog.
		public int Prop => 0;
	}
}

/// A quick brown fox jumped over the lazy dog.