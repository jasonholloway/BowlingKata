using NUnit.Framework;
using FsCheck;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bowling.Test
{
    public struct Score
    {
        public readonly int Value;

        public Score(int value) {
            Value = value;
        }
    }



    public class Game
    {
        public int Score { get; private set; }

        public int Frame { get; private set; } = 0;
        public int Turn { get; private set; } = 0;

        public void Roll(int struckPins) {
            if(Frame >= 10) throw new Exception("Can't roll any more! Exceeded maximum number of frames!");

            Score += struckPins;

            if(struckPins >= 10) {
                Turn = 0;
                Frame++;
            }
            else {
                if(++Turn >= 2) {
                    Turn = 0;
                    Frame++;
                }
            }
        }
    }

    public static class GameExtensions
    {        
        public static void RollMany(this Game game, IEnumerable<int> rollScores) {
            foreach(var rollScore in rollScores) {
                game.Roll(rollScore);
            }
        }

        public static void RollMany(this Game game, params int[] rollScores)
            => game.RollMany(rollScores.AsEnumerable());
    }




    [TestFixture]
    public class BowlingTests
    {
        Game _game;

        [SetUp]
        public void SetUp() {
            _game = new Game();
        }
        
        [Test]
        public void SingleRoll_UpdatesScore() {
            _game.Roll(5);
            _game.Score.ShouldBe(5);
        }

        [Test]
        public void ManyRolls_UpdateScore() {
            _game.RollMany(1, 2, 3, 4, 5);
            _game.Score.ShouldBe(1 + 2 + 3 + 4 + 5);
        }

        [Test]
        public void Frame_StartsAtZero() {
            _game.Frame.ShouldBe(0);
        }

        [Test]
        public void Frame_AdvancesEveryTwoRolls_GivenInabilityToBowl() {
            _game.RollMany(Enumerable.Repeat(0, 10));
            _game.Frame.ShouldBe(5);
        }

        [Test]
        public void Frame_AdvancesDirectly_GivenScoreOfTen() {
            _game.Roll(10);
            _game.Frame.ShouldBe(1);
        }

        [Test]
        public void Frame_CantGoPastTen() {
            Should.Throw<Exception>(
                () => _game.RollMany(Enumerable.Repeat(0, 21)));
        }
        

    }
}
