using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
                var chancesOfNextWeekPattern = currentWeek.CalculateChanceOfNextWeekPattern();

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

        public MainWindow()
        {
            InitializeComponent();

            onlyEnableSundaysOnCalendar();
            populateComboBoxes();
        }
    }
}
