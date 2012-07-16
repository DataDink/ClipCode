using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ClipCode
{
    /// <summary>
    /// Parses Visual Studio clipboard RTF ONLY. This is not a fully-featured RTF parser <br />
    /// </summary>
    public class VsRtfParser
    {
        private VsRtfParser() {}
        /// <summary>
        /// Parses Visual Studio clipboard RTF ONLY. This is not a fully-featured RTF parser <br />
        /// This also makes a number of assumptions about VS rtf formatting. <br />
        /// Please report bugs to https://github.com/datadink mailto:markerdink@gmail.com
        /// </summary>
        public static string ParseToHtml(string rawRtf)
        {
            var lines = new List<string>();

            // Reading colors from header
            var colors = new List<string>(new[] {"#000000"});
            var colorHeader = Regex.Match(rawRtf, "\\{\\\\colortbl.+?\\}");
            var colorValues = Regex.Matches(colorHeader.Value, "\\\\red(?<red>\\d+)\\\\green(?<green>\\d+)\\\\blue(?<blue>\\d+)");
            colors.AddRange(colorValues.OfType<Match>()
                .Select(color => Color.FromArgb(255,
                    int.Parse(color.Groups["red"].Value),
                    int.Parse(color.Groups["green"].Value),
                    int.Parse(color.Groups["blue"].Value)))
                    .Select(ColorTranslator.ToHtml));

            // Assuming color to be the end of header
            var headerLength = colorHeader.Index + colorHeader.Length;
            var terminator = rawRtf.LastIndexOf('}') - headerLength; // Assuming terminating bracket
            rawRtf = rawRtf.Substring(headerLength, terminator);

            // Removing escapes on brackets
            rawRtf = ReplaceCode(rawRtf, "{", "{");
            rawRtf = ReplaceCode(rawRtf, "}", "}");
            rawRtf = ReplaceCode(rawRtf, "par ", "\r\n"); // Adding real line-breaks
            string line;
            using (var reader = new StringReader(rawRtf))
            while ((line = reader.ReadLine()) != null)
            {
                // Make html friendly
                line = line.Replace("&", "&amp;")
                    .Replace("<", "&lt;")
                    .Replace(">", "&gt;")
                    .Replace(" ", "&nbsp;");

                // Handle the codes we care about... (Assuming a space after /codes)
                line = ReplaceCode(line, "tab&nbsp;", "&nbsp;&nbsp;&nbsp;&nbsp;");
                line = ReplaceCode(line, "cf(\\d+)&nbsp;", "<span class=\"c$1\">"); // Will close these later
                line = ReplaceCode(line, "[a-zA-Z0-9]+&nbsp;", ""); // We don't care about anything else.
                    
                // Now we can close color spans
                line = Regex.Replace(line, "(<span class=\"c\\d+\">.*?)(?=<span class=\"c\\d+\">)", "$1</span>");
                if (Regex.IsMatch(line, "<span class=\"c\\d+\">"))
                    line += "</span>";
                lines.Add(line);
            }

            // Contain the code in a div with overflow set to auto and a max-height
            var result = "<div style=\"overflow: auto; max-height: 600px; border: 1px dotted #808080; margin: 25px;\">\r\n"
                         + "\t<style>";
            // Create an inline styling for the code block
            result += ".clip-code {font-family: Menlo, Monaco, Consolas, \"Courier New\", monospace;"
                      + "font-size: 11px;"
                      + "padding: 5px;"
                      + "border: 1px dotted #404040;"
                      + "word-wrap: break-word;"
                      + "list-style: decimal outside none;"
                      + "margin: 0px 0px 0px 33px;}";
            var colorIndex = 0;
            colors.ForEach(c => result += ".clip-code .c" + colorIndex++ + "{color: " + c + ";}");
            result += "</style>\r\n";
            // Add each line as a ordered-list item (so we get line numbers)
            result += "\t<ol class=\"clip-code\">\r\n";
            lines.ForEach(l => result += "\t\t<li>" + l + "</li>\r\n");
            result += "\t</ol>\r\n</div>";
            return result;
        }

        private static string ReplaceCode(string line, string code, string replace)
        {
            // In most usages of this a space is being assumed after each /code
            return Regex.Replace(line, "(?<!\\\\)\\\\" + code, replace);
        }
    }
}
