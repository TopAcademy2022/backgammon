using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace BackGammon
{
    public class GameGrid : Grid
    {
        private GameField _gameField;

        // Because 7 is center - black line
        private const uint _COUNT_COLLS = 13;

        private const uint _COUNT_ROWS = 24;

        List<Rectangle> _allPosiblePositions;

        GameFigure _currentSelectedFigure;

        // получаем ЛОГИЧЕСКУЮ позицию фигуры, не на гриде, а в массиве логики
        private uint[] GetFigurePosition(UIElement gameFigure)
        {
            uint yCoordinateCurrentButton = Convert.ToUInt32(Grid.GetColumn(gameFigure));

            uint offset = (yCoordinateCurrentButton > 6) ? 1 : (uint)0;

            return new uint[] { Convert.ToUInt32(Grid.GetRow(gameFigure)), yCoordinateCurrentButton - offset };
        }

        private void RemoveAllPosiblePosition()
        {
            foreach (Rectangle posiblePosition in this._allPosiblePositions)
            {
                this.Children.Remove(posiblePosition);
            }
        }

        private void EnterPosiblePosition(object sender, MouseButtonEventArgs mouseEvent)
        {
            // Получаем позицию элемента, куда был клик
            Rectangle posiblePosition = sender as Rectangle;
            this._gameField.MoveFigureToPosition(this.GetFigurePosition(this._currentSelectedFigure), this.GetFigurePosition(posiblePosition));
            int xCoordinateCurrentClick = Grid.GetRow(posiblePosition);
            int yCoordinateCurrentClick = Grid.GetColumn(posiblePosition);

            this.RemoveAllPosiblePosition();

            // Переставляем нашу клавишу
            Grid.SetRow(this._currentSelectedFigure, xCoordinateCurrentClick);
            Grid.SetColumn(this._currentSelectedFigure, yCoordinateCurrentClick);
        }

        private void PrintFigureMovementPosition(List<uint[]> positions)
        {
            // ОЧИСТИТЬ ПРЕДЫДУЩИЕ ПРЯМОУГОЛЬНИКИ. P.s. через лист, так как ещё ниже в собитии дёргать в листе
            this.RemoveAllPosiblePosition();

            foreach (uint[] position in positions)
            {
                Rectangle rectangle = new Rectangle() { Fill = new SolidColorBrush() { Color = Color.FromRgb(255, 0, 0) } };
                this._allPosiblePositions.Add(rectangle);
                this.Children.Add(rectangle);
                Grid.SetRow(rectangle, (int)position[0]);

                uint offset = (position[1] >= 6) ? 1 : (uint)0;
                Grid.SetColumn(rectangle, (int)(position[1] + offset));

                rectangle.MouseLeftButtonDown += EnterPosiblePosition;
            }
        }

        public GameGrid(Window? parentElement = null)
        {
            this._gameField = new GameField();
            this._allPosiblePositions = new List<Rectangle>();

            this.Background = new ImageBrush();

            if (parentElement != null)
            {
                parentElement.Content = this;
            }
            for (uint i = 0; i < _COUNT_ROWS; i++)
            {
                this.RowDefinitions.Add(new RowDefinition());
            }
            for (uint i = 0; i < _COUNT_COLLS; i++)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        public void RenderGameField()
        {
            GameField.FillGameField[,] fillGameFields = this._gameField.GetGameField();

            for (int i = 0; i < _COUNT_ROWS; i++)
            {
                for (int k = 0, indexGridColl = 0; k < _COUNT_COLLS - 1; k++, indexGridColl++)
                {
                    if (indexGridColl == 7)
                    {
                        indexGridColl++;
                    }
                    if (fillGameFields[i, k] == GameField.FillGameField.WhiteFigure)
                    {
                        //поставь белую кнопку в позицию, но не ту!
                        GameFigure gameFigure = new GameFigure(GameFigure.FigureColor.White);

                        this.Children.Add(gameFigure);
                        Grid.SetRow(gameFigure, Convert.ToInt32(i));
                        Grid.SetColumn(gameFigure, indexGridColl);
                    }
                    else if (fillGameFields[i, k] == GameField.FillGameField.BlackFigure)
                    {
                        GameFigure gameFigure = new GameFigure(GameFigure.FigureColor.Black);

                        this.Children.Add(gameFigure);
                        Grid.SetRow(gameFigure, Convert.ToInt32(i));
                        Grid.SetColumn(gameFigure, indexGridColl);
                    }
                }
            }
        }

        public void PrintPossibleMoves(GameFigure gameFigure)
        {
            uint[] cubes = this._gameField.GetRandomCubes();
            MessageBox.Show($"Куб1 = {cubes[0]}, куб2 = {cubes[1]}");
            if (this._gameField.CheckFigureWasMovement(this.GetFigurePosition(gameFigure)))
            {
                this._currentSelectedFigure = gameFigure;
                this.PrintFigureMovementPosition(this._gameField.GetFigureMovementPositions());
            }
            else
            {
                this.RemoveAllPosiblePosition();
            }
        }
    }
}
