using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ACNH_TurnipCalculator.Enums;

namespace ACNH_TurnipCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Week currentWeek = new Week();

        private void textBoxNumberValidation(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void onlyEnableSundaysOnCalendar()
        {
            var minDate = dpBuyingDate.DisplayDateStart ?? DateTime.MinValue;
            var maxDate = dpBuyingDate.DisplayDateEnd ?? DateTime.MaxValue;

            var firstSunday = findNextSunday(minDate);

            if (minDate != firstSunday)
            {
                var range = new CalendarDateRange(minDate, firstSunday.Subtract(new TimeSpan(1, 0, 0, 0, 0)));
                dpBuyingDate.BlackoutDates.Add(range);
            }

            for (var day = firstSunday.AddDays(1); day <= maxDate; day = day.AddDays(7))
            {
                var range = new CalendarDateRange(day, day.AddDays(5));
                dpBuyingDate.BlackoutDates.Add(range);
            }
        }

        private DateTime findNextSunday(DateTime date)
        {
            for (var day = date; day <= date.AddDays(7); day = day.AddDays(1))
            {
                if (day.DayOfWeek == DayOfWeek.Sunday)
                {
                    return day;
                }
            }
            return date;
        }

        private void populateComboBoxes()
        {
            cmbPreviousPattern.ItemsSource = Enum.GetValues(typeof(Pattern));
            cmbPreviousPattern.SelectedItem = Pattern.Unkown;

            cmbFirstTimeBuyer.ItemsSource = Enum.GetValues(typeof(FirstTimeBuyer));
            cmbFirstTimeBuyer.SelectedItem = FirstTimeBuyer.No;
        }

        private void updatePreviousWeekPattern(object sender, SelectionChangedEventArgs e)
        {
            if (cmbPreviousPattern.SelectedItem == null || cmbFirstTimeBuyer.SelectedItem == null)
            {
                return;
            }
            else if (cmbFirstTimeBuyer.SelectedItem.ToString() == FirstTimeBuyer.Yes.ToString())
            {
                lbPreviousWeek.Text = Pattern.SmallSpike.ToString();
                currentWeek.LastWeekPattern = Pattern.SmallSpike;
                cmbPreviousPattern.SelectedItem = Pattern.Unkown;
                cmbPreviousPattern.IsEnabled = false;
            }
            else
            {
                lbPreviousWeek.Text = cmbPreviousPattern.SelectedItem.ToString();
                currentWeek.LastWeekPattern = (Pattern)cmbPreviousPattern.SelectedItem;
                cmbPreviousPattern.IsEnabled = true;
            }

            calculateChancesOfNextWeekPattern();
        }

        private void calculateChancesOfNextWeekPattern()
        {
            if (cmbPreviousPattern.SelectedItem.ToString() != Pattern.Unkown.ToString())
            {
                var chancesOfNextWeekPattern = currentWeek.CalculateChancesOfNextWeekPattern();

                int randomChance;
                chancesOfNextWeekPattern.TryGetValue(Pattern.Random, out randomChance);
                lbRandomChance.Text = randomChance.ToString() + "%";

                int largeSpikeChance;
                chancesOfNextWeekPattern.TryGetValue(Pattern.LargeSpike, out largeSpikeChance);
                lbLargeSpikeChance.Text = largeSpikeChance.ToString() + "%";

                int decreasingChance;
                chancesOfNextWeekPattern.TryGetValue(Pattern.Decreasing, out decreasingChance);
                lbDecreasingChance.Text = decreasingChance.ToString() + "%";

                int smallSpikeChance;
                chancesOfNextWeekPattern.TryGetValue(Pattern.SmallSpike, out smallSpikeChance);
                lbSmallSpikeChance.Text = smallSpikeChance.ToString() + "%";
            }
            else
            {
                lbRandomChance.Text = "-";
                lbLargeSpikeChance.Text = "-";
                lbDecreasingChance.Text = "-";
                lbSmallSpikeChance.Text = "-";
            }
        }

        private void updateBasePrice(object sender, TextChangedEventArgs e)
        {
            int basePrice;

            try
            {
                basePrice = int.Parse(tbBasePrice.Text);
            }
            catch (Exception)
            {
                lbBasePrice.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                lbBasePrice.Text = "TBD";
                return;
            }

            lbBasePrice.Text = basePrice.ToString();
            currentWeek.BasePrice = basePrice;

            if (basePrice < 90 || basePrice > 110)
            {
                lbBasePrice.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                lbBasePrice.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }
        }

        private void updateBuyingDate(object sender, SelectionChangedEventArgs e)
        {
            lbBuyingDate.Text = String.Format("{0:dd/MMM/yyyy}", dpBuyingDate.SelectedDate);
            currentWeek.BuyingDate = dpBuyingDate.SelectedDate;
        }

        private int readPrice(TextBox textBox)
        {
            if (textBox.Text == "")
            {
                return 0;
            }

            try
            {
                return int.Parse(textBox.Text);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void btCalculate_Click(object sender, RoutedEventArgs e)
        {
            currentWeek.SetPrice(HalfDay.MondayAM,readPrice(tbMondayAMPrice));
            currentWeek.SetPrice(HalfDay.MondayPM,readPrice(tbMondayPMPrice));
            currentWeek.SetPrice(HalfDay.TuesdayAM,readPrice(tbTuesdayAMPrice));
            currentWeek.SetPrice(HalfDay.TuesdayPM,readPrice(tbTuesdayPMPrice));
            currentWeek.SetPrice(HalfDay.WednesdayAM,readPrice(tbWednesdayAMPrice));
            currentWeek.SetPrice(HalfDay.WednesdayPM,readPrice(tbWednesdayPMPrice));
            currentWeek.SetPrice(HalfDay.ThursdayAM,readPrice(tbThursdayAMPrice));
            currentWeek.SetPrice(HalfDay.ThursdayPM,readPrice(tbThursdayPMPrice));
            currentWeek.SetPrice(HalfDay.FridayAM,readPrice(tbFridayAMPrice));
            currentWeek.SetPrice(HalfDay.FridayPM,readPrice(tbFridayPMPrice));
            currentWeek.SetPrice(HalfDay.SaturdayAM,readPrice(tbSaturdayAMPrice));
            currentWeek.SetPrice(HalfDay.SaturdayPM,readPrice(tbSaturdayPMPrice));

            int price = currentWeek.GetPrice(HalfDay.MondayAM);
            lbMonAM.Text = price == 0 ? "-" : price.ToString();
            price = currentWeek.GetPrice(HalfDay.MondayPM);
            lbMonPM.Text = price == 0 ? "-" : price.ToString();
            price = currentWeek.GetPrice(HalfDay.TuesdayAM);
            lbTueAM.Text = price == 0 ? "-" : price.ToString();
            price = currentWeek.GetPrice(HalfDay.TuesdayPM);
            lbTuePM.Text = price == 0 ? "-" : price.ToString();
            price = currentWeek.GetPrice(HalfDay.WednesdayAM);
            lbWedAM.Text = price == 0 ? "-" : price.ToString();
            price = currentWeek.GetPrice(HalfDay.WednesdayPM);
            lbWedPM.Text = price == 0 ? "-" : price.ToString();
            price = currentWeek.GetPrice(HalfDay.ThursdayAM);
            lbThuAM.Text = price == 0 ? "-" : price.ToString();
            price = currentWeek.GetPrice(HalfDay.ThursdayPM);
            lbThuPM.Text = price == 0 ? "-" : price.ToString();
            price = currentWeek.GetPrice(HalfDay.FridayAM);
            lbFriAM.Text = price == 0 ? "-" : price.ToString();
            price = currentWeek.GetPrice(HalfDay.FridayPM);
            lbFriPM.Text = price == 0 ? "-" : price.ToString();
            price = currentWeek.GetPrice(HalfDay.SaturdayAM);
            lbSatAM.Text = price == 0 ? "-" : price.ToString();
            price = currentWeek.GetPrice(HalfDay.SaturdayPM);
            lbSatPM.Text = price == 0 ? "-" : price.ToString();

            Movement movement = currentWeek.GetMovement(HalfDay.MondayPM);
            lbMovementMonPM.Text = movement == Movement.Increasing ? "Inc" : "Dec";
            movement = currentWeek.GetMovement(HalfDay.TuesdayAM);
            lbMovementTueAM.Text = movement == Movement.Increasing ? "Inc" : "Dec";
            movement = currentWeek.GetMovement(HalfDay.TuesdayPM);
            lbMovementTuePM.Text = movement == Movement.Increasing ? "Inc" : "Dec";
            movement = currentWeek.GetMovement(HalfDay.WednesdayAM);
            lbMovementWedAM.Text = movement == Movement.Increasing ? "Inc" : "Dec";
            movement = currentWeek.GetMovement(HalfDay.WednesdayPM);
            lbMovementWedPM.Text = movement == Movement.Increasing ? "Inc" : "Dec";
            movement = currentWeek.GetMovement(HalfDay.ThursdayAM);
            lbMovementThuAM.Text = movement == Movement.Increasing ? "Inc" : "Dec";
            movement = currentWeek.GetMovement(HalfDay.ThursdayPM);
            lbMovementThuPM.Text = movement == Movement.Increasing ? "Inc" : "Dec";
            movement = currentWeek.GetMovement(HalfDay.FridayAM);
            lbMovementFriAM.Text = movement == Movement.Increasing ? "Inc" : "Dec";
            movement = currentWeek.GetMovement(HalfDay.FridayPM);
            lbMovementFriPM.Text = movement == Movement.Increasing ? "Inc" : "Dec";
            movement = currentWeek.GetMovement(HalfDay.SaturdayAM);
            lbMovementSatAM.Text = movement == Movement.Increasing ? "Inc" : "Dec";
            movement = currentWeek.GetMovement(HalfDay.SaturdayPM);
            lbMovementSatPM.Text = movement == Movement.Increasing ? "Inc" : "Dec";
        }

        public MainWindow()
        {
            InitializeComponent();

            onlyEnableSundaysOnCalendar();
            populateComboBoxes();
        }
    }
}
