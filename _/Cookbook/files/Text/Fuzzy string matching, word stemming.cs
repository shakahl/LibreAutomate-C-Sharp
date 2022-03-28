/// Use fuzzy string matching to compare or find approximately matching words, for example misspelled. We'll use library <link https://github.com/JakeBayer/FuzzySharp>FuzzySharp<>. NuGet package <+nuget>FuzzySharp<>.
///
/// Use word stemming (removing suffix) to find all word forms (with/without any suffix). We'll use library <link https://github.com/nemec/porter2-stemmer>porter2-stemmer<>. NuGet package <+nuget>Porter2Stemmer<>. English only.

/*/ nuget -\FuzzySharp; nuget -\Porter2Stemmer; /*/

/// Get % of similarity.

string text = "National Geographic";
string word1 = "nation", word2 = "geografy";

print.it(FuzzySharp.Fuzz.PartialRatio(text, word1, FuzzySharp.PreProcess.PreprocessMode.Full));
print.it(FuzzySharp.Fuzz.PartialRatio(text, word2, FuzzySharp.PreProcess.PreprocessMode.Full));

/// The FuzzySharp web page gives code examples for most functions.

/// Get word stem.

var stemmer = new Porter2Stemmer.EnglishPorter2Stemmer();
print.it(stemmer.Stem("friend").Value);
print.it(stemmer.Stem("friendly").Value);
print.it(stemmer.Stem("friends").Value);
print.it(stemmer.Stem("friend's").Value);
