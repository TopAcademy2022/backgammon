using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace BackGammon
{
    public class GameFigure : Button
    {
        private uint _width;

        private uint _height;

        private FigureColor _figureColor;

        public enum FigureColor: byte
        {
            White = 0,
            Black = 1
        }

        public GameFigure(FigureColor figureColor)
        {
            this._width = 30;
            this._height = 30;
            this._figureColor = figureColor;
            DefaultStyleKey = typeof(GameFigure);
            this.Click += OnButtonClick;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            SolidColorBrush brush = null;

            if (this._figureColor == FigureColor.White)
            {
                brush = new SolidColorBrush(Colors.White);
            }
            else
            {
                brush = new SolidColorBrush(Colors.Black);
            }

            double radius = 10;
            double borderThickness = 1;
            drawingContext.DrawRoundedRectangle(brush, new Pen(brush, borderThickness), new Rect(0, 0, this._width, this._height), radius, radius);
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            GameFigure? currentButton = sender as GameFigure;
            GameGrid? ourGrid = currentButton.Parent as GameGrid;
            ourGrid.PrintPossibleMoves(currentButton);
        }
    }
}
