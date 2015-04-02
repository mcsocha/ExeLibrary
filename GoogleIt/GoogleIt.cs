namespace GoogleIt
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Web;

    class GoogleIt
    {
        static void Main(string[] args)
        {

            var debug = false;
            var debugOutPath = Path.Combine(Environment.CurrentDirectory, "googleIt.log");
            var debugOut = "";
            var quotePlaceholder = Guid.NewGuid().ToString("N");
            var plusPlaceholder = Guid.NewGuid().ToString("N");
            
            debugOut += "# args: " + args.Length;

            var q = "";
            for(int i = 0;i < args.Length; i++)
            {
                bool haswsp = ContainsWhitespace(args[i]);

                if(haswsp) q += quotePlaceholder;
                    
                q+= args[i];

                if (haswsp) q += quotePlaceholder;

                if(i+1 != args.Length)
                    q += plusPlaceholder;
            }
            q = HttpUtility.UrlEncode(q);
            q = q.Replace(quotePlaceholder, "%22").Replace(plusPlaceholder, "+");

            debugOut += "\r\nq: [" + q + "]";
            var url = string.Format("http://www.google.com/search?q={0}", q);

            debugOut += "\r\nurl: [" + url + "]";

            Process.Start(url);

            if(debug)
            {
                var writer = new StreamWriter(debugOutPath, false);
                writer.WriteLine(debugOut);

                writer.Dispose();
            }
            
        }

        static bool ContainsWhitespace(string input)
        {
            bool contains = input.Any(c => Char.IsWhiteSpace(c));
            return contains;
        }
    }
}