using System;
using System.Collections.Generic;
using System.Linq;
using Me.Bartecki.Allegro.Domain.Services.Interfaces;

namespace Me.Bartecki.Allegro.Domain.Services
{
    public class LetterCounterService : ILetterCounterService
    {
        public Dictionary<char, int> CountLetters(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Input string cannot be null or whitespace", nameof(input));

            var charArray = input
                .ToLower()
                .ToCharArray();
            var letters = charArray
                .Where(c => char.IsLetter(c))
                .GroupBy(c => c)
                .ToDictionary(x => x.Key, x => x.Count());
            return letters;
        }
    }
}
