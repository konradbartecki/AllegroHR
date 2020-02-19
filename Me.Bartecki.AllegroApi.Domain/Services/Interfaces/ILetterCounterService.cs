using System.Collections.Generic;

namespace Me.Bartecki.Allegro.Domain.Services.Interfaces
{
    public interface ILetterCounterService
    {
        Dictionary<char, int> CountLetters(string input);
    }
}