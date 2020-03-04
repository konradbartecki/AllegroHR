using Me.Bartecki.Allegro.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Me.Bartecki.Allegro.Domain.UnitTests
{
    [TestClass]
    public class LetterCounterTests
    {
        [TestMethod]
        public void CanCountLetters()
        {
            //Arrange
            string input = "allegro";
            var service = new LetterCounterService();

            //Act
            var letterCount = service.CountLetters(input);

            //Assert
            Assert.AreEqual(6, letterCount.Count);
            Assert.AreEqual(1, letterCount['a']);
            Assert.AreEqual(2, letterCount['l']);
            Assert.AreEqual(1, letterCount['e']);
            Assert.AreEqual(1, letterCount['g']);
            Assert.AreEqual(1, letterCount['r']);
            Assert.AreEqual(1, letterCount['o']);
        }

        [TestMethod]
        public void CanIgnoreSpecialCharacters()
        {
            //Arrange
            string input = "a-$#a";
            var service = new LetterCounterService();

            //Act
            var letterCount = service.CountLetters(input);

            //Assert
            Assert.AreEqual(1, letterCount.Count);
            Assert.AreEqual(2, letterCount['a']);
        }

        [TestMethod]
        public void CanIgnoreNumbers()
        {
            //Arrange
            string input = "aa1231";
            var service = new LetterCounterService();

            //Act
            var letterCount = service.CountLetters(input);

            //Assert
            Assert.AreEqual(1, letterCount.Count);
            Assert.AreEqual(2, letterCount['a']);
        }

        [TestMethod]
        public void CanIgnoreCase()
        {
            //Arrange
            string input = "AAaA";
            var service = new LetterCounterService();

            //Act
            var letterCount = service.CountLetters(input);

            //Assert
            Assert.AreEqual(1, letterCount.Count);
            Assert.AreEqual(4, letterCount['a']);
        }

        [TestMethod]
        public void CanIgnoreWhitespace()
        {
            //Arrange
            string input = "a a";
            var service = new LetterCounterService();

            //Act
            var letterCount = service.CountLetters(input);

            //Assert
            Assert.AreEqual(1, letterCount.Count);
            Assert.AreEqual(2, letterCount['a']);
        }

        [TestMethod]
        public void Throws_ArgumentException_OnNull()
        {
            //Arrange
            var service = new LetterCounterService();

            //Act
            Assert.ThrowsException<ArgumentException>(() => service.CountLetters(null));
        }

        [TestMethod]
        public void Throws_ArgumentException_OnWhitespace()
        {
            //Arrange
            var service = new LetterCounterService();

            //Act
            Assert.ThrowsException<ArgumentException>(() => service.CountLetters("   "));
        }


    }
}
