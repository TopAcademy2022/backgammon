using System;
using System.Collections.Generic;

namespace BackGammon
{
    public class Dice
    {
        public bool IsDoubleDice { get; private set; }

        public List<uint> RandomCubeValues { get; init; }

        public Dice()
        {
            this.RandomCubeValues = new List<uint>();
            this.UpdateDice();
        }

        // Перебросить кубики
        public void UpdateDice()
        {
            Random random = new Random();
            this.RandomCubeValues.Clear();

            uint firstRandomCube = 4;
            uint secondRandomCube = 4;

            if (firstRandomCube != secondRandomCube)
            {
                this.IsDoubleDice = false;
                this.RandomCubeValues.Add(firstRandomCube);
                this.RandomCubeValues.Add(secondRandomCube);
                this.RandomCubeValues.Add(firstRandomCube + secondRandomCube);
            }
            else
            {
                this.IsDoubleDice = true;
                for (int i = 1; i <= 4; i++)
                {
                    this.RandomCubeValues.Add((uint)(firstRandomCube * i));
                }
            }
        }
    }
}
