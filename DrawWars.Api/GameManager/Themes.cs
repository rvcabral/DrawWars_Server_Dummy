using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace DrawWars.Api.GameManager
{
    public static class Themes
    {
        public static bool IsCorrect(string Theme, string Guess)
        {
            string theme = RemoveDiacritics(Theme).ToLower().Trim();
            string guess = RemoveDiacritics(Guess).ToLower().Trim();
            if (guess == theme) return true;

            var themeWords = theme.Split(" ").Where(t => t.Length > 2).ToArray();
            var guessWords = guess.Split(" ").Where(g => g.Length > 2).ToArray();

            if (themeWords.Length != guessWords.Length) return false;

            foreach(var word in guessWords)
            {
                if (!themeWords.Contains(word))
                    return false;
            }

            return true;
        }
        
        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
