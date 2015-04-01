using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegTool
{
    public static class Util
    {
        public static bool ContainsCaseInsensitive(this string input, string search)
        {
            return input.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string ReplaceCaseInsensitive(this string input, string token, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = input.ToUpper();
            string upperPattern = token.ToUpper();
            int inc = (input.Length / token.Length) *
                      (replacement.Length - token.Length);
            char[] chars = new char[input.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern,
                                              position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = input[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + token.Length;
            }
            if (position0 == 0) return input;
            for (int i = position0; i < input.Length; ++i)
                chars[count++] = input[i];
            return new string(chars, 0, count);
        }
    }
}
