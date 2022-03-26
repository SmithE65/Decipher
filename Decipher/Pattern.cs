namespace Decipher;

public class Pattern
{
    private readonly ushort[] _pattern;

    public int Length => _pattern.Length;

    public Pattern(string s)
    {
        _pattern = new ushort[s.Length];
        ParseInternal(s);
    }

    public int this[int idx] => _pattern[idx];

    public static bool operator ==(Pattern a, Pattern b)
    {
        if (a.Length != b.Length)
        {
            return false;
        }

        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i])
            {
                return false;
            }
        }

        return true;
    }

    public static bool operator !=(Pattern a, Pattern b)
    {
        return !(a == b);
    }

    public int[] GetValue() => _pattern.Select(x => (int)x).ToArray();

    private void ParseInternal(string s)
    {
        var d = new Dictionary<char, ushort>();
        ushort idx = 0;

        for (int i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (d.TryGetValue(c, out var v))
            {
                _pattern[i] = v;
            }
            else
            {
                d.Add(c, idx);
                _pattern[i] = idx++;
            }
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is Pattern p)
        {
            return this == p;
        }

        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        return _pattern.GetHashCode();
    }
}
