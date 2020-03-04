﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Me.Bartecki.Allegro.Domain.Model;
using Me.Bartecki.Allegro.Domain.Services;
using Me.Bartecki.Allegro.Domain.Services.Decorators;
using Me.Bartecki.Allegro.Domain.Services.Interfaces;
using Me.Bartecki.Allegro.Infrastructure.Integrations.RepositoryStores;
using Me.Bartecki.Allegro.Infrastructure.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Optional;

namespace Me.Bartecki.Allegro.Domain.UnitTests
{
    /// <summary>
    /// What could also be tested:
    ///  - negative forks or other properties
    ///  - null repository list
    /// </summary>
    [TestClass]
    public class RepoStatisticsTests
    {
        [TestMethod]
        public void CanCountRepos()
        {
            //Arrange
            var mockedLetterCounter = new Mock<ILetterCounterService>();
            var mockedRepositoryStore = new Mock<IRepositoryStoreService>();
            var mockedRepositories = new List<Repository>()
            {
                new Repository()
                {
                    Name = "Allegro-Api",
                    Forks = 1,
                    Stargazers = 1,
                    Size = 1,
                    Watchers = 1
                },
                new Repository()
                {
                    Name = "Allegro-test",
                    Forks = 3,
                    Stargazers = 7,
                    Size = 15,
                    Watchers = 31
                },
            };

            mockedLetterCounter.Setup(x => x.CountLetters(It.IsAny<string>()))
                .Returns(new Dictionary<char, int>());
            mockedRepositoryStore.Setup(x => x.GetUserRepositoriesAsync("username"))
                .ReturnsAsync(Option.Some<IEnumerable<Repository>, AllegroApiException>(mockedRepositories));
            var service = new RepoStatisticsService(mockedRepositoryStore.Object, mockedLetterCounter.Object);
            
            //Act
            var stats = service.GetRepositoryStatisticsAsync("username").Result
                .Match(some => some, none => null);

            //Assert
            Assert.AreEqual(2, stats.AverageForks);
            Assert.AreEqual(4, stats.AverageStargazers);
            Assert.AreEqual(8, stats.AverageSize);
            Assert.AreEqual(16, stats.AverageWatchers);
        }

        [TestMethod]
        public void CanCountRepos_WithZeroForks()
        {
            //Arrange
            var mockedLetterCounter = new Mock<ILetterCounterService>();
            var mockedRepositoryStore = new Mock<IRepositoryStoreService>();
            var mockedRepositories = new List<Repository>()
            {
                new Repository()
                {
                    Name = "Allegro-Api",
                    Forks = 0
                },
                new Repository()
                {
                    Name = "Allegro-test",
                    Forks = 0
                },
            };

            mockedLetterCounter.Setup(x => x.CountLetters(It.IsAny<string>()))
                .Returns(new Dictionary<char, int>());
            mockedRepositoryStore.Setup(x => x.GetUserRepositoriesAsync("username"))
                .ReturnsAsync(Option.Some<IEnumerable<Repository>, AllegroApiException>(mockedRepositories));
            var service = new RepoStatisticsService(mockedRepositoryStore.Object, mockedLetterCounter.Object);

            //Act
            var stats = service.GetRepositoryStatisticsAsync("username").Result
                .Match(some => some, none => null);

            //Assert
            Assert.AreEqual(0, stats.AverageForks);
        }

        [TestMethod]
        public void CanSumAllLettersUsed()
        {
            //Arrange
            var mockedLetterCounter = new Mock<ILetterCounterService>();
            var mockedRepositoryStore = new Mock<IRepositoryStoreService>();

            var repos = new List<Repository>()
            {
                new Repository() {Name = "Allegro-api"},
                new Repository() {Name = "Allegro-test"}
            };
            mockedRepositoryStore.Setup(x => x.GetUserRepositoriesAsync("username"))
                .ReturnsAsync(Option.Some<IEnumerable<Repository>, AllegroApiException>(repos));
            mockedLetterCounter.Setup(x => x.CountLetters("Allegro-api"))
                .Returns(new Dictionary<char, int>()
                {
                    ['a'] = 2,
                    ['b'] = 3
                });
            mockedLetterCounter.Setup(x => x.CountLetters("Allegro-test"))
                .Returns(new Dictionary<char, int>()
                {
                    ['a'] = 3,
                    ['x'] = 2
                });
            var service = new RepoStatisticsService(mockedRepositoryStore.Object, mockedLetterCounter.Object);

            //Act
            var stats = service.GetRepositoryStatisticsAsync("username").Result
                .Match(some => some, none => null);

            //Assert
            stats.Letters.Should().BeEquivalentTo(new Dictionary<char, int>()
            {
                ['a'] = 5,
                ['b'] = 3,
                ['x'] = 2
            }, "should be able to sum all chars used");
            mockedLetterCounter.Verify(x => x.CountLetters("Allegro-api"), Times.Once);
            mockedLetterCounter.Verify(x => x.CountLetters("Allegro-test"), Times.Once);
        }

        [TestMethod]
        public void CanSafelyIgnoreUsersWithoutRepos()
        {
            //Arrange
            var mockedLetterCounter = new Mock<ILetterCounterService>();
            var mockedRepositoryStore = new Mock<IRepositoryStoreService>();
            mockedRepositoryStore.Setup(x => x.GetUserRepositoriesAsync("username"))
                .ReturnsAsync(Option.Some<IEnumerable<Repository>, AllegroApiException>(new List<Repository>()));

            var service = new RepoStatisticsService(mockedRepositoryStore.Object, mockedLetterCounter.Object);

            //Act
            var stats = service.GetRepositoryStatisticsAsync("username").Result;

            //Assert
            stats.MatchNone(exception => Assert.AreEqual(ErrorCodes.UserHasNoRepositories, exception.ErrorCode));
        }

        [TestMethod]
        public void Decorator_CanReturnOrderedDictionary()
        {
            //Arrange
            var mockedOutput = new UserStatistics()
            {
                Letters = new Dictionary<char, int>()
                {
                    ['c'] = 10,
                    ['b'] = 3,
                    ['a'] = 15
                }
            };
            var innerService = new Mock<IRepoStatisticsService>();
            innerService.Setup(x => x.GetRepositoryStatisticsAsync(It.IsAny<string>()))
                .ReturnsAsync(Option.Some<UserStatistics, AllegroApiException>(mockedOutput));
            var decorator = new OrderedLettersStatisticsDecorator(innerService.Object);

            //Act
            var orderedOutput = decorator.GetRepositoryStatisticsAsync("").Result
                .Match(some => some, none => null);

            //Assert
            Assert.AreEqual('a', orderedOutput.Letters.First().Key);
            Assert.AreEqual('c', orderedOutput.Letters.Last().Key);
        }
    }
}
