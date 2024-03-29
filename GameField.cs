﻿using System;
using System.Collections.Generic;

namespace BackGammon
{
    /*!
     *  @brief Logic of game.
     */
    public class GameField
    {
        /*!
        *  @brief Fill game field.
        */
        public enum FillGameField : byte
        {
            Empty = 0, ///< Field cell not fill
            WhiteFigure = 1, ///< Field cell fill white figure
            BlackFigure = 2 ///< Field cell fill black figure
        }

        private const uint _COUNT_MAX_FIGURES = 15;

        private uint _countDeletedWhiteFigure; //удаленные белые

        private uint _countDeletedBlackFigure; //удаленные черные

        private uint _firstRandomCubeValue;

        private uint _secondRandomCubeValue;

        private const uint _COUNT_COLLS = 12;

        private const uint _COUNT_ROWS = 24;

        private FillGameField[,] _gameField;

        private bool _whiteFiguresIsWalking;

        private List<uint[]> _figureMovementPositions;
        
        private bool ChecEndGame()
        {
            // убрать магическое число 15 на константу
            if (this._countDeletedWhiteFigure == _COUNT_MAX_FIGURES || this._countDeletedBlackFigure == _COUNT_MAX_FIGURES)
            {
                return true;
            }

            return false;
        }

        private bool DetermineWhiteVictoryFigure()
        {
            if (this._countDeletedWhiteFigure == _COUNT_MAX_FIGURES)
            {
                return true;
            }

            return false;
        }


        private FillGameField GetFigureByPosition(uint[] figurePosition)
        {
            return this._gameField[figurePosition[0], figurePosition[1]];
        }

        private bool CheckPlayerAccessFigure(uint[] figurePosition)
        {
            if (this.GetFigureByPosition(figurePosition) == FillGameField.WhiteFigure && this._whiteFiguresIsWalking ||
                this.GetFigureByPosition(figurePosition) == FillGameField.BlackFigure && !this._whiteFiguresIsWalking)
            {
                return true;
            }

            return false;
        }

        /*!
        * @brief Determining the position of the piece at the top or bottom of the playing field.
        * @param [in] Any figure that requires position determination in {x, y} format.
        * Where x is a row and y is a column in the game board.
        * @return True is UP; False is DOWN .
         */
        private bool GetFigureDirectionMovement(uint[] figurePosition)
        {
            uint[] startPositionMovemet = new uint[2] { 0, figurePosition[1] };

            for (uint i = 0; startPositionMovemet[0] + i < figurePosition[0]; i++)
            {
                startPositionMovemet[0] += i;

                if (GetFigureByPosition(startPositionMovemet) == FillGameField.Empty)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckFigureIsBlocked(uint[] figurePosition)
        {
            // TODO: Понять где верх
            bool startFigureIsUp = this.GetFigureDirectionMovement(figurePosition);

            // TODO: Обработать out of range
            uint[] figureUpPosition = null;

            if (figurePosition[0] < _COUNT_ROWS - 1)
            {
                figureUpPosition = new uint[2] { figurePosition[0] + 1, figurePosition[1] };
            }
            else
            {
                figureUpPosition = new uint[2] { figurePosition[0], figurePosition[1] };
            }

            // TODO: Обработать out of range
            uint[] figureDownPosition = null;

            if (figurePosition[0] != 0)
            {
                figureDownPosition = new uint[2] { figurePosition[0] - 1, figurePosition[1] };
            }
            else
            {
                figureDownPosition = new uint[2] { figurePosition[0], figurePosition[1] };
            }

            if (startFigureIsUp && this.GetFigureByPosition(figureUpPosition) == FillGameField.Empty ||
                !startFigureIsUp && this.GetFigureByPosition(figureDownPosition) == FillGameField.Empty)
            {
                return false;
            }
            return true;
        }

        /*!
        *  @brief Prohibition On Moving Figure After Returning Home.
        *  @param[in]  positionOfFigureOnField - A variable representing the position of a piece on the board.
        *  @param[in]  figurePositionColls - A variable representing the position of a shape along a line.
        *  @param[in]  cubeValue - Variable representing the value of the die
        *  @return true - Access is allowed, false - Access blocked
        */
        private bool СheckingToSeeIfWeCanGoIfWeGetHome (uint positionOfFigureOnField, uint figurePositionColls, uint cubeValue)
        {
            uint tempNumberOfMoves = 0;

            if(this._whiteFiguresIsWalking)
            {
                if( positionOfFigureOnField == 0)
                {
                    for(int i = (int)figurePositionColls; i > 0; i--)
                    {
                        tempNumberOfMoves++;
                    }

                    tempNumberOfMoves += _COUNT_COLLS;

                    if(tempNumberOfMoves >= cubeValue)
                    {
                        return true;
                    }
                }
                else
                {
                    for(uint i = figurePositionColls; i < _COUNT_COLLS - 1; i++)
                    {
                        tempNumberOfMoves++;
                    }

                    if(tempNumberOfMoves >= cubeValue)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if(positionOfFigureOnField == 23)
                {
                    for(uint i = figurePositionColls; i < _COUNT_COLLS - 1; i++)
                    {
                        tempNumberOfMoves++;
                    }

                    tempNumberOfMoves += _COUNT_COLLS;

                    if(tempNumberOfMoves >= cubeValue)
                    {
                        return true;
                    }
                }
                else
                {
                    for(int i = ( int ) figurePositionColls; i >= 0; i--)
                    {
                        tempNumberOfMoves++;
                    }

                    if(tempNumberOfMoves >= cubeValue)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private uint[]? СalculateFigureMovementOnePosition(uint[] figurePosition, uint cubeValue)
        {
            // самая итоговая позиция, куда мы можем походить
            uint[] figureMovementOnePosition = null;

            bool startFigureIsUp = this.GetFigureDirectionMovement(figurePosition);

            // При старте пробуем поставить в начальную строку
            uint startFigureRow = (startFigureIsUp == true) ? (uint)0 : 23;
            // итоговая позиция куда ставить фигуру
            uint[] figureNewFirstPosition = null;
            if(СheckingToSeeIfWeCanGoIfWeGetHome( startFigureRow, figurePosition[1], cubeValue))
            {
                // Двигаемся по столбцам влево или вправо (если сверху, то влево. если снизу, то вправо)
                if(startFigureIsUp)
                {
                    // Вычисляем не пережли ли мы в другую сторону доски
                    if((int)figurePosition[1] - cubeValue < 0)
                    {
                        // инверсия строки из-за переноса в другую сторону
                        uint finishFigureRow = (startFigureRow == 0) ? 23 : (uint)0;
                        // вычислить сдвиг по столбцам
                        uint finishFigureColl = 0;
                        if( (int) figurePosition[1] - cubeValue < 0)
                        {
                            finishFigureColl = (uint)((-((int)figurePosition[1] - cubeValue))- 1);
                        }
                        else
                        {
                            finishFigureColl = figurePosition[1] - cubeValue;
                        }

                        figureNewFirstPosition = new uint[2] {finishFigureRow, finishFigureColl};
                    }
                    else
                    {
                        // нет инверсии строки, т.к. нет перехода в другую часть поля
                        // поэтому строка "изначальная" = startFigureRow
                        figureNewFirstPosition = new uint[2] {startFigureRow, figurePosition[1] - cubeValue};
                    }
                }
                else
                {
                    // Вычисляем не пережли ли мы в другую сторону доски
                    if(figurePosition[1] + cubeValue > _COUNT_COLLS - 1)
                    {
                        uint finishFigureRow = (startFigureRow == 0) ? 23 : (uint)0;
                        uint finishFigureColl = _COUNT_COLLS - 1 - (figurePosition[1] + cubeValue) % _COUNT_COLLS;

                        figureNewFirstPosition = new uint[2] {finishFigureRow, finishFigureColl};
                    }
                    else
                    {
                        // нет инверсии строки, т.к. нет перехода в другую часть поля
                        // поэтому строка "изначальная" = startFigureRow
                        figureNewFirstPosition = new uint[2] {startFigureRow, figurePosition[1] + cubeValue};
                    }
                }
                // Если цвет одинаковый, то ставь сверху
                if(this.GetFigureByPosition(figureNewFirstPosition) == this.GetFigureByPosition(figurePosition))
                {
                    uint newRow = 0;
                    // назначаем новую строку
                    // если наша новая позиция сверху->вниз
                    if(this.GetFigureDirectionMovement(figureNewFirstPosition))
                    {
                        for(uint i = 1; i < _COUNT_ROWS; i++)
                        {
                            if(this.GetFigureByPosition(new uint[2] {i, figureNewFirstPosition[1]}) == FillGameField.Empty)
                            {
                                newRow = i;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for(uint i = _COUNT_ROWS - 2; i >= 0; i--)
                        {
                            if(this.GetFigureByPosition(new uint[2] {i, figureNewFirstPosition[1]}) == FillGameField.Empty)
                            {
                                newRow = i;
                                break;
                            }
                        }
                    }

                    figureMovementOnePosition = new uint[2] {newRow, figureNewFirstPosition[1]};
                }
                else if(this.GetFigureByPosition(figureNewFirstPosition) == FillGameField.Empty)
                {
                    figureMovementOnePosition = figureNewFirstPosition;
                }
            }
            return figureMovementOnePosition;
        }

        private List<uint[]> СalculateFigureMovementPositions(uint[] figurePosition)
        {
            List<uint[]?> allFigureMovementPositions = new List<uint[]>();

            // Понимаем сколько вариантов может быть
            if (this._firstRandomCubeValue != this._secondRandomCubeValue)
            {
                allFigureMovementPositions.Add(СalculateFigureMovementOnePosition(figurePosition, this._firstRandomCubeValue));
                allFigureMovementPositions.Add(СalculateFigureMovementOnePosition(figurePosition, this._secondRandomCubeValue));
                allFigureMovementPositions.Add(СalculateFigureMovementOnePosition(figurePosition, this._firstRandomCubeValue + this._secondRandomCubeValue));
            }
            else ///< если вариантов 4 (значение кубика 1 и 2 равны)
            {
                for (uint i = 1; i < 5; i++)
                {
                    allFigureMovementPositions.Add(СalculateFigureMovementOnePosition(figurePosition, this._firstRandomCubeValue * i));
                }
            }

            allFigureMovementPositions.RemoveAll(item => item == null);

            return allFigureMovementPositions;
        }

        private bool ContainsFigureMovementPosition(uint[] figurePosition)
        {
            foreach (uint[] figureMovementPosition in this._figureMovementPositions)
            {
                if (figureMovementPosition[0] == figurePosition[0] &&
                    figureMovementPosition[1] == figurePosition[1])
                {
                    return true;
                }
            }
            return false;
        }

        public GameField()
        {
            const uint COUNT_FIGURES = 15;

            this._whiteFiguresIsWalking = this.DefineWalkingWhiteFigure();
            this._gameField = new FillGameField[_COUNT_ROWS, _COUNT_COLLS];
            this._figureMovementPositions = new List<uint[]>();

            for (uint i = 0; i < _COUNT_ROWS; i++)
            {
                for (uint k = 0; k < _COUNT_COLLS; k++)
                {
                    this._gameField[i, k] = FillGameField.Empty;
                }
            }

            for (uint i = _COUNT_ROWS - 1; i >= (_COUNT_ROWS - COUNT_FIGURES); i--)
            {
                this._gameField[i, 0] = FillGameField.BlackFigure;
            }

            for (uint i = 0; i < COUNT_FIGURES; i++)
            {
                this._gameField[i, _COUNT_COLLS - 1] = FillGameField.WhiteFigure;
            }
        }

        public FillGameField[,] GetGameField()
        {
            return this._gameField;
        }

        // Получить значение кубиков
        public uint[] GetRandomCubes()
        {
            return new uint[2] { this._firstRandomCubeValue, this._secondRandomCubeValue };
        }

        // первый проброс, чтобы найти ходящего
        private bool DefineWalkingWhiteFigure()
        {
            Random random = new Random();

            do
            {
                this._firstRandomCubeValue = 1;
                this._secondRandomCubeValue = 5;

                if (this._firstRandomCubeValue > this._secondRandomCubeValue)
                {
                    return true;
                }
            }
            while (this._firstRandomCubeValue == this._secondRandomCubeValue);

            return false;
        }

        // Перебросить кубики
        private void UpdateCubes()
        {
            Random random = new Random();
            this._firstRandomCubeValue = Convert.ToUInt32(random.Next(1, 7));
            this._secondRandomCubeValue = Convert.ToUInt32(random.Next(1, 7));
        }

        /*!
        *  @brief Checks if the selected figure can go anywhere.
        *  @param [in] startFigurePosition The position of the selected figure in {x, y} format.
        *  Where x is a row and y is a column in the game board.
        *  @return Possibility of movement.
        */
        public bool CheckFigureWasMovement(uint[] figurePosition)
        {
            if (this.CheckPlayerAccessFigure(figurePosition)) ///< Проверяем, может ли текущий пользователь двигать фигуру
            {
                if (!this.CheckFigureIsBlocked(figurePosition)) ///< Проверяем, не заблокирована ли фигура
                {
                    this._figureMovementPositions = this.СalculateFigureMovementPositions(figurePosition); ///< Получаем позиции, куда фигура может двигаться
                    /*
                     * @if Number of possible moves > 0
                     * That movement is possible.
                     * @endif
                    */
                    if (this._figureMovementPositions.Count > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /*!
        *  @brief Get positions of all points where the player's piece can go.
        *  @return Positions where the previously selected piece can go (from the method of checking access to movement).
        *  The position of the figure is in the format {x, y}, where x is a row and y is a column.
        */
        public List<uint[]> GetFigureMovementPositions()
        {
            return this._figureMovementPositions;
        }

        public bool MoveFigureToPosition(uint[] startFigurePosition, uint[] finishFigurePosition)
        {
            if (this.ContainsFigureMovementPosition(finishFigurePosition))
            {
                // заливаем нашим цветом финишную
                this._gameField[finishFigurePosition[0], finishFigurePosition[1]] = this._gameField[startFigurePosition[0], startFigurePosition[1]];
                // обнуляем стартовую
                this._gameField[startFigurePosition[0], startFigurePosition[1]] = FillGameField.Empty;
                // ходит другой игрок
                this._whiteFiguresIsWalking = !this._whiteFiguresIsWalking;
                // перебрасываем кубики
                this.UpdateCubes();
                return true;
            }

            return false;
        }
    }
}
