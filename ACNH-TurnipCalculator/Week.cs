using System;
using System.Collections.Generic;
using System.Text;
using ACNH_TurnipCalculator.Enums;

namespace ACNH_TurnipCalculator
{
    public class Week
    {
        public const int TOTAL_HALF_DAYS = 12;

        private int[] prices = new int[TOTAL_HALF_DAYS];

        private Movement[] movements = new Movement[TOTAL_HALF_DAYS];

        public FirstTimeBuyer FirstTimeBuyer { get; set; }

        public Pattern LastWeekPattern { get; set; }

        public int BasePrice { get; set; }

        public DateTime? BuyingDate { get; set; }

        public void SetPrice(HalfDay halfDay, int price)
        {
            prices[(int)halfDay] = price;
            recalculateMovements();
        }

        public int GetPrice(HalfDay halfDay)
        {
            return prices[(int)halfDay];
        }

        public Movement GetMovement(HalfDay halfDay)
        {
            return movements[(int)halfDay];
        }

        private void recalculateMovements()
        {
            for (int i = 1; i < prices.Length; i++)
            {
                if (prices[i - 1] != 0 && prices[i] != 0)
                {
                    movements[i] = prices[i - 1] < prices[i] ? Movement.Increasing : Movement.Decreasing;
                }
            }
        }

        public Dictionary<Pattern, int> CalculateChancesOfNextWeekPattern()
        {
            var chancesOfNextPattern = new Dictionary<Pattern, int>();

            switch (LastWeekPattern)
            {
                case Pattern.Random:
                    chancesOfNextPattern.Add(Pattern.Random, 20);
                    chancesOfNextPattern.Add(Pattern.LargeSpike, 30);
                    chancesOfNextPattern.Add(Pattern.Decreasing, 15);
                    chancesOfNextPattern.Add(Pattern.SmallSpike, 35);
                    break;
                case Pattern.LargeSpike:
                    chancesOfNextPattern.Add(Pattern.Random, 50);
                    chancesOfNextPattern.Add(Pattern.LargeSpike, 5);
                    chancesOfNextPattern.Add(Pattern.Decreasing, 20);
                    chancesOfNextPattern.Add(Pattern.SmallSpike, 25);
                    break;
                case Pattern.Decreasing:
                    chancesOfNextPattern.Add(Pattern.Random, 25);
                    chancesOfNextPattern.Add(Pattern.LargeSpike, 45);
                    chancesOfNextPattern.Add(Pattern.Decreasing, 5);
                    chancesOfNextPattern.Add(Pattern.SmallSpike, 25);
                    break;
                case Pattern.SmallSpike:
                    chancesOfNextPattern.Add(Pattern.Random, 45);
                    chancesOfNextPattern.Add(Pattern.LargeSpike, 25);
                    chancesOfNextPattern.Add(Pattern.Decreasing, 15);
                    chancesOfNextPattern.Add(Pattern.SmallSpike, 15);
                    break;
                default:
                    break;
            }

            return chancesOfNextPattern;
        }
    }
}
