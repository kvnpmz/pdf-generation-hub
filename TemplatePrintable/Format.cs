using System.Text.RegularExpressions;

public static class Format
{
    public static string Beautify(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var outList = new List<string>();
        int indent = 0;

        var regex = new Regex(@"(<[^>]*>)|([^<]+)", RegexOptions.Compiled);
        var matches = regex.Matches(html);

        for (int i = 0; i < matches.Count; i++)
        {
            var match = matches[i];

            if (match.Groups[1].Success)
            {
                string tag = match.Groups[1].Value;
                bool isClosing = tag.StartsWith("</");
                bool isSelfClosing = tag.EndsWith("/>");

                if (!isClosing)
                {
                    bool inline = !isSelfClosing && IsSimpleInline(matches, i);

                    if (inline)
                    {
                        string content = "";
                        int j = i + 1;

                        for (; j < matches.Count; j++)
                        {
                            if (matches[j].Groups[2].Success)
                                content += matches[j].Value;
                            else
                                break;
                        }

                        string closing = matches[j].Value;

                        outList.Add(
                            new string(' ', indent * 4) +
                            tag + content.Trim() + closing
                        );

                        i = j;
                        continue;
                    }

                    if (!isClosing)
                    {
                        outList.Add(new string(' ', indent * 4) + tag);

                        if (!isSelfClosing && tag.ToLower() != "<html>" && tag.ToLower() != "<body>" && tag.ToLower() != "<head>")
                            indent++;
                    }
                }

                else
                {
                    indent = Math.Max(indent - 1, 0);
                    outList.Add(new string(' ', indent * 4) + tag);
                }
            }

            else if (match.Groups[2].Success)
            {
                string text = match.Groups[2].Value.Trim();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    outList.Add(new string(' ', indent * 4) + text);
                }
            }
        }

        return string.Join(Environment.NewLine, outList);
    }

    private static bool IsSimpleInline(MatchCollection matches, int index)
    {
        for (int i = index + 1; i < matches.Count; i++)
        {
            var m = matches[i];

            if (m.Groups[1].Success)
            {
                string tag = m.Groups[1].Value;

                if (tag.StartsWith("</"))
                    return true;

                return false;
            }
        }

        return false;
    }
}
