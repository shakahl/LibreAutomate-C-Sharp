/*
The source of cookbook folders and recipes is workspace "Cookbook" in app folder.
The program presents it as the cookbook tree: folder -> folder, script -> recipe, others ignored.
When a node is selected in the tree, the program parses the script and presents as a recipe.
A script can contain doc comments and codes.
Doc comments (/// TEXT and /** TEXT) are rendered as text.
	Can contain <see> and Au output tags.
	Must be at the start of a line.
Other code is rendered as code. Including //comments.
	If { CODE } or if(...){ CODE }, only CODE is used. At the start of a line. In if-CODE removes 1 tab indentation.
Code before the first doc comment is not rendered. It can be used for testing code examples when writing the script.
	For example can contain using directives or set a variable to select if-CODE block(s) to test.
	If there are no doc comments, entire script is used as code.
Below is an example script.
*/

int e = 2;

/// A quick brown <b>fox</b> jumped over the lazy dog.
/// See also <see cref="print.it"/> and <see cref="Console.Write"/>.
print.it("text");

/// A quick brown fox jumped over the lazy dog.
/// A quick brown fox jumped over the lazy dog.
{
print.it("text");
}
/// A quick brown fox jumped over the lazy dog.
/// A quick brown fox jumped over the lazy dog.
if(e==2) {
	print.it(4);
	print.it(4);
}
/// A quick brown fox jumped over the lazy dog.

/// A quick brown fox jumped over the lazy dog.

//Local functions are code examples too (not only body).
void Local(int i) {
	print.it(1);
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