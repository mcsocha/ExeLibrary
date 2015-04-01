using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class RegexEscape
{
    /// <summary>
    /// RegEx Escape.  Replaces text in your clipboard with its regex-escaped literal form.
    /// </summary>
    /// <param name="args"></param>
    [STAThread]
    public static void Main(string[] args)
    {
        Console.WriteLine("Regex Escaper");

        if(!Clipboard.ContainsText())
            Console.WriteLine("Your clipboard must contain text for this to work!");
        else
        {
            var text = Clipboard.GetText();
            Console.WriteLine(string.Format("Your clipboard contained the following text: {0}", text));

            //not using Regex.Escape() because it's incompatible with grepWin's regex version
            text = text.Replace("\\", "\\\\").Replace("$", "\\$").Replace(".", "\\.").Replace("^", "\\^")
                .Replace("(", "\\(").Replace(")", "\\)").Replace("{", "\\{").Replace("}", "\\}").Replace("*", "\\*")
                .Replace("\r\n", "\\r\\n").Replace("?", "\\?").Replace("[", "\\[").Replace("]", "\\]");

            Clipboard.SetText(text);
            Console.WriteLine(string.Format("Your clipboard's text was Regex escaped to: {0}", text));
        }

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey(true);
    }
}
