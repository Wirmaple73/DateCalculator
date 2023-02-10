using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace DateCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PersianCalendar pc = new PersianCalendar();
        UmAlQuraCalendar hc = new UmAlQuraCalendar();  // Seems to be more accurate than HijriCalendar (?)
        GregorianCalendar gc = new GregorianCalendar();

        DateTime dt, dt2;
        TimeSpan ts;

        #region Date Converter

        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            
            PerformConversions();
        }

        private void PerformConversions()
        {
            try
            {
                switch (CBoxConvertFromType.SelectedIndex)
                {
                    case 0:  // Persian
                        ConvertFromDate(pc);
                        break;

                    case 1:  // Hijri
                        ConvertFromDate(hc);
                        break;

                    case 2:  // Gregorian
                        ConvertFromDate(gc);
                        break;

                    default:
                        throw new ArgumentNullException();
                }

                DisplayConversions();
            }
            catch
            {
                EmptyConversions();
                ShowMbInvalidDate();
            }
        }

        private void ConvertFromDate(Calendar calType)
        {
            dt = new DateTime(int.Parse(TBoxYear.Text), int.Parse(CBoxMonth.SelectedValue.ToString()), int.Parse(CBoxDay.SelectedValue.ToString()), calType);
        }

        private void DisplayConversions()
        {
            LabelToPersian.Content = string.Format("{0}/{1}/{2}", pc.GetYear(dt), pc.GetMonth(dt), pc.GetDayOfMonth(dt));
            LabelToHijri.Content = string.Format("{0}/{1}/{2}", hc.GetYear(dt), hc.GetMonth(dt), hc.GetDayOfMonth(dt));
            LabelToGregorian.Content = string.Format("{0}/{1}/{2}", gc.GetYear(dt), gc.GetMonth(dt), gc.GetDayOfMonth(dt));
        }

        private void EmptyConversions()
        {
            LabelToPersian.Content = "~";
            LabelToHijri.Content = "~";
            LabelToGregorian.Content = "~";
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (sender == BtnCopyPersian)
            {
                CopyToClipboard(LabelToPersian.Content.ToString());
            }
            else if (sender == BtnCopyHijri)
            {
                CopyToClipboard(LabelToHijri.Content.ToString());
            }
            else
            {
                CopyToClipboard(LabelToGregorian.Content.ToString());
            }
        }

        #endregion
        #region Date Difference Calculator

        private void BtnCalcDifference_Click(object sender, RoutedEventArgs e)
        {
            GetDateDifference();
        }

        private void GetDateDifference()
        {
            try
            {
                switch (CBoxDiffFromType.SelectedIndex)
                {
                    case 0:  // Persian
                        GetDiffFromDate(pc); break;

                    case 1:  // Hijri
                        GetDiffFromDate(hc); break;

                    case 2:  // Gregorian
                        GetDiffFromDate(gc); break;

                    default:
                        throw new ArgumentNullException();
                }

                SubtractDates();
                LabelDateDifference.Content = string.Format("{0} روز ({1:F2} سال)", Math.Abs(ts.TotalDays), Math.Abs(ts.TotalDays / 365));
            }
            catch
            {
                LabelDateDifference.Content = "~";
                ShowMbInvalidDate();
            }
        }

        private void GetDiffFromDate(Calendar calType)
        {
            dt = new DateTime(int.Parse(TBoxDiffYearFrom.Text), int.Parse(CBoxDiffMonthFrom.SelectedValue.ToString()), int.Parse(CBoxDiffDayFrom.SelectedValue.ToString()), calType);
            dt2 = new DateTime(int.Parse(CBoxDiffYearTo.Text), int.Parse(CBoxDiffMonthTo.SelectedValue.ToString()), int.Parse(CBoxDiffDayTo.SelectedValue.ToString()), calType);
        }

        private void SubtractDates()
        {
            ts = dt.Subtract(dt2);
        }

        #endregion
        #region Date Addition/Subtraction Calculator

        private void BtnAddSubDate_Click(object sender, RoutedEventArgs e)
        {
            AddOrSubFromDate();
        }

        private void AddOrSubFromDate()
        {
            try
            {
                switch (CBoxAddSubType.SelectedIndex)
                {
                    case 0:  // Persian
                        AddSubGetDates(pc); break;

                    case 1:  // Hijri
                        AddSubGetDates(hc); break;

                    case 2:  // Gregorian
                        AddSubGetDates(gc); break;

                    default:
                        throw new ArgumentNullException();
                }

                AddSubGetNewDates();
                GetAddSubResults();
            }
            catch
            {
                LabelDateAfterAddSub.Content = "~";
                ShowMbInvalidDate();
            }
        }

        private void AddSubGetDates(Calendar calType)
        {
            dt = new DateTime(int.Parse(TBoxAddSubYear.Text), int.Parse(CBoxAddSubMonth.SelectedValue.ToString()), int.Parse(CBoxAddSubDay.SelectedValue.ToString()), calType);
        }

        private void AddSubGetNewDates()
        {
            int years = Math.Abs(int.Parse(TBoxSelAddSubYear.Text)),
            months = Math.Abs(int.Parse(TBoxSelAddSubMonth.Text)),
            days = Math.Abs(int.Parse(TBoxSelAddSubDay.Text));


            if (RBtnAddToDate.IsChecked == true)
            {
                AddOrSubDate(years, months, days);  // Add to date
            }
            else
            {
                AddOrSubDate(-years, -months, -days);  // Subtract from date
            }
        }

        private void AddOrSubDate(int years, int months, int days)
        {
            dt = dt.AddYears(years);
            dt = dt.AddMonths(months);
            dt = dt.AddDays(days);
        }

        private void GetAddSubResults()
        {
            switch (CBoxAddSubType.SelectedIndex)
            {
                case 0:  // Persian
                    DisplayAddSubResults(pc.GetYear(dt), pc.GetMonth(dt), pc.GetDayOfMonth(dt)); break;

                case 1:  // Hijri
                    DisplayAddSubResults(hc.GetYear(dt), hc.GetMonth(dt), hc.GetDayOfMonth(dt)); break;

                case 2:  // Gregorian
                    DisplayAddSubResults(gc.GetYear(dt), gc.GetMonth(dt), gc.GetDayOfMonth(dt)); break;
            }
        }

        private void DisplayAddSubResults(int year, int month, int day)
        {
            LabelDateAfterAddSub.Content = string.Format("{0}/{1}/{2}", year, month, day);
        }

        private void BtnCopyAddSubDate_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(LabelDateAfterAddSub.Content.ToString());
        }

        #endregion

        #region Miscellaneous

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillCBoxValues();
        }

        private void FillCBoxValues()
        {
            for (byte i = 1; i <= 31; i++)  // Days
            {
                CBoxDay.Items.Add(i);

                CBoxDiffDayFrom.Items.Add(i);
                CBoxDiffDayTo.Items.Add(i);

                CBoxAddSubDay.Items.Add(i);
            }

            for (byte i = 1; i <= 12; i++)  // Months
            {
                CBoxMonth.Items.Add(i);

                CBoxDiffMonthFrom.Items.Add(i);
                CBoxDiffMonthTo.Items.Add(i);

                CBoxAddSubMonth.Items.Add(i);
            }
        }

        private void ShowMbInvalidDate()
        {
            MessageBox.Show(".لطفاً یک تاریخ معتبر را وارد کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.RightAlign);
        }

        private void CopyToClipboard(string str)
        {
            Clipboard.SetText(str);
        }

        private void AboutLink_Click(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Wirmaple73");
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }

        #endregion
    }
}