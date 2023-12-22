namespace BackGammon.Tests
{
    public class GameFieldTests
    {
        private readonly BackGammon.GameField _gameField = new GameField();

        [Fact]
        public void DefineWalkingWhiteFigureTest()
        {
            bool act = this._gameField.DefineWalkingWhiteFigure();
            Assert.True(act);
        }
    }
}