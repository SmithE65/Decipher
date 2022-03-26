// See https://aka.ms/new-console-template for more information
using Decipher;

Console.WriteLine("Hello, World!");

var path = args[0];

//if (!Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute))
//{
//    throw new Exception("Invalid file path");
//}

var stream = File.OpenText(path);
var words = ParseFile(stream);
stream.Close();

Console.WriteLine($"{words.Count} distinct word lengths.");

var istream = File.OpenText(args[1]);
var (cipher, ciphered) = ProcessInput(istream);
istream.Close();

Console.WriteLine($"{ciphered.Count} distinct words in cipher.");
//foreach (var w in ciphered.OrderByDescending(x => x.Length))
//{
//    Console.WriteLine(w);
//}

//var longest = ciphered.MaxBy(x => x.Length) ?? string.Empty;
//var longestPattern = new Pattern(longest);
//Console.WriteLine($"Longest word in cipher: {longest}, {string.Join(", ", new Pattern(longest).GetValue().Select(x => x.ToString()))}");

//var sameLength = words[longest.Length];
//Console.WriteLine($"Words of same length: {sameLength.Count}");
//var patternMatch = sameLength.Where(x => new Pattern(x) == longestPattern);
//Console.WriteLine($"Pattern match count: {patternMatch.Count()}");
//foreach (var w in patternMatch)
//{
//    Console.WriteLine(w);
//}

//var key = new Dictionary<char, char>();
//var guess = patternMatch.ElementAt(6);
//for (int i = 0; i < longest.Length; i++)
//{
//    key.TryAdd(longest[i], guess[i]);
//}

Dictionary<char, char> mostKeys = new();
Console.WriteLine();
var (_, key) = DecipherText(words, new(), ciphered.OrderByDescending(x => x.Length).ToList(), ref mostKeys);

Console.WriteLine(cipher);
var pass1 = cipher.Select(x => Decipher(x, key ?? mostKeys));
Console.WriteLine(new string(pass1.ToArray()));
var pass2 = cipher.Select(x => Decipher(x, mostKeys));
Console.WriteLine(new string(pass2.ToArray()));

foreach (var c in ciphered)
{
    var word = new string(c.Select(x => Decipher(x, mostKeys)).ToArray());
    var isWord = words[word.Length].Any(x => x == word);
    Console.WriteLine($"{word}: {isWord}");
}

static Dictionary<int, List<string>> ParseFile(StreamReader reader)
{
    var words = new Dictionary<int, List<string>>();
    var wordCount = 0;

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();

        if (string.IsNullOrWhiteSpace(line))
        {
            continue;
        }

        if (line.Any(x => !char.IsLetter(x)))
        {
            continue;
        }

        line = line.Trim().ToUpper();
        var length = line.Length;

        if (!words.ContainsKey(length))
        {
            words[length] = new List<string> { line };
        }
        else
        {
            words[length].Add(line);
        }
        wordCount++;
    }

    Console.WriteLine($"{wordCount} words read from file.");
    return words;
}

static (string Cipher, HashSet<string> Words) ProcessInput(StreamReader reader)
{
    var text = reader.ReadToEnd() ?? string.Empty;
    var processed = new string(text.Where(c => char.IsLetter(c) || char.IsWhiteSpace(c)).ToArray());
    var split = processed.ToUpper().Split(' ');
    return (text, new HashSet<string>(split));
}

static char Decipher(char c, Dictionary<char,char> key)
{
    if (!char.IsLetter(c))
    {
        return c;
    }

    return key.TryGetValue(char.ToUpper(c), out var v) == true ? v : '_';
}

static (bool IsSuccess, Dictionary<char, char>? Key) DecipherText(Dictionary<int, List<string>>  dictionary, Dictionary<char,char> key, List<string> words, ref Dictionary<char,char> max)
{
    var word = words.First();
    var pattern = new Pattern(word);
    var sameLength = dictionary[word.Length];
    var patternMatch = sameLength.Where(x => new Pattern(x) == pattern && KeyCheck(x, word, key));

    if (!patternMatch.Any())
    {
        //return (false, null);
        return DecipherText(dictionary, key, words.Skip(1).ToList(), ref max);
    }

    foreach (var p in patternMatch)
    {
        var (success, clone) = AddKeys(word, p, key);
        if (clone.Count > max.Count)
        {
            max = clone;
        }

        if (!success)
        {
            continue;
        }

        if (words.Count > 1)
        {
            var (IsSuccess, Key) = DecipherText(dictionary, clone, words.Skip(1).ToList(), ref max);

            if (IsSuccess)
            {
                return (true, Key);
            }
        }
    }

    return (false, key);
}

static (bool Success, Dictionary<char,char> Keys) AddKeys(string s1, string s2, Dictionary<char,char> key)
{
    var clone = new Dictionary<char, char>(key);
    for (int i = 0; i < s1.Length; i++)
    {
        if (clone.ContainsKey(s1[i]) && clone[s1[i]] != s2[i])
        {
            return (false, clone);
        }
        clone.TryAdd(s1[i], s2[i]);
    }
    return (true, clone);
}

static bool KeyCheck(string s1, string s2, Dictionary<char,char> key)
{
    for (int i = 0; i < s1.Length; i++)
    {
        if (key.ContainsKey(s2[i]) && key[s2[i]] != s1[i])
        {
            return false;
        }
    }
    return true;
}