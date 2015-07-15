namespace RegexEscape
{
    using System;
    using System.Windows.Forms;

    class RegexEscape
    {
        /// <summary>
        /// RegEx Escape.  Replaces text in your clipboard with its regex-escaped literal form. (Perl Regex)
        /// </summary>
        /// <param name="args"></param>
        [STAThread] //STAThread needed for clipboard access.
        static void Main(string[] args)
        {
            Console.WriteLine("Regex Escaper");

            if(!Clipboard.ContainsText())
                Console.WriteLine("Your clipboard must contain text for this to work!");
            else
            {
                var text = Clipboard.GetText();
                Console.WriteLine(string.Format("Your clipboard contained the following text: {0}", text));

                //http://regexhero.net/reference/

                text = text
                    .Replace("\\", "\\\\")
                    .Replace(".", "\\.")
                    .Replace("$", "\\$")                    
                    .Replace("^", "\\^")
                    .Replace("{", "\\{")
                    .Replace("[", "\\[")
                    .Replace("(", "\\(")
                    .Replace("|", "\\|")
                    .Replace(")", "\\)")
                    .Replace("*", "\\*")
                    .Replace("+", "\\+")
                    .Replace("?", "\\?")
                    .Replace("\r\n", "\\r\\n");

                Clipboard.SetText(text);
                Console.WriteLine(string.Format("Your clipboard's text was Regex escaped to: {0}", text));
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey(true);
        }
    }
}