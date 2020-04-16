using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace ACNH_TurnipCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            OnlyEnableSundaysOnCalendar();
        }

        private void OnlyEnableSundaysOnCalendar()
        {
            var minDate = dpBuyingDate.DisplayDateStart ?? DateTime.MinValue;
            var maxDate = dpBuyingDate.DisplayDateEnd ?? DateTime.MaxValue;

            var firstSunday = FindNextSunday(minDate);

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

        private DateTime FindNextSunday(DateTime date)
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
    }
}
