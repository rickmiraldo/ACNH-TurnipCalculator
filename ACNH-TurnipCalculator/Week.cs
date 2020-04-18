using System;
using System.Collections.Generic;
using System.Text;
using ACNH_TurnipCalculator.Enums;

namespace ACNH_TurnipCalculator
{
    public class Week
    {
        public FirstTimeBuyer FirstTimeBuyer { get; set; }

        public Pattern LastWeekPattern { get; set; }

        public int BasePrice { get; set; }

        public DateTime? BuyingDate { get; set; }

        public int MondayAM { get; set; }

        public int MondayPM { get; set; }

        public int TuesdayAM { get; set; }

        public int TuesdayPM { get; set; }

        public int WednesdayAM { get; set; }

        public int WednesdayPM { get; set; }

        public int ThursdayAM { get; set; }

        public int ThursdayPM { get; set; }

        public int FridayAM { get; set; }

        public int FridayPM { get; set; }

        public int SaturdayAM { get; set; }

        public int SaturdayPM { get; set; }

        public Dictionary<Pattern, int> CalculateChanceOfNextWeekPattern()
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
