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
        DCalculator calc = new DCalculator();
        const string nullStr = "~";

        public MainWindow()
        {
            InitializeComponent();
        }


        // --- Date Converter ---

        private void ConCalc_Convert_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                calc.ConvertDate(Convert.ToInt32(ConCalc_TBoxYear.Text), Convert.ToInt32(ConCalc_CBoxMonth.Text), Convert.ToInt32(ConCalc_CBoxDay.Text), GetCalendar(ConCalc_CBoxConFromType.SelectedIndex));
            }
            catch
            {
                ConCalc_DisplayConversions(nullStr, nullStr, nullStr);

                ShowMboxError();
                return;
            }

            ConCalc_DisplayConversions(calc.ResultPC, calc.ResultQc, calc.ResultGC);
        }

        public void ConCalc_DisplayConversions(string resultPc, string resultQc, string resultGc)
        {
            ConCalc_LabelPersian.Content = resultPc;
            ConCalc_LabelHijri.Content = resultQc;
            ConCalc_LabelGregorian.Content = resultGc;
        }

        private void ConCalc_SetTodayDate_Click(object sender, RoutedEventArgs e)
        {
            calc.SetTodayDate();
            ConCalc_DisplayConversions(calc.ResultPC, calc.ResultQc, calc.ResultGC);
        }

        private void ConCalc_CopyDate_Click(object sender, RoutedEventArgs e)
        {
            if (sender == ConCalc_CopyPersian)
            {
                CopyToClipboard(ConCalc_LabelPersian.Content.ToString());
            }
            else if (sender == ConCalc_CopyHijri)
            {
                CopyToClipboard(ConCalc_LabelHijri.Content.ToString());
            }
            else
            {
                CopyToClipboard(ConCalc_LabelGregorian.Content.ToString());
            }
        }


        // --- Date Difference Calculator ---

        private void DiffCalc_Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                calc.CalculateDifference(Convert.ToInt32(DiffCalc_TBoxYear1.Text), Convert.ToInt32(DiffCalc_CBoxMonth1.Text), Convert.ToInt32(DiffCalc_CBoxDay1.Text), Convert.ToInt32(DiffCalc_TBoxYear2.Text), Convert.ToInt32(DiffCalc_CBoxMonth2.Text), Convert.ToInt32(DiffCalc_CBoxDay2.Text), GetCalendar(DiffCalc_CBoxConFromType.SelectedIndex));
            }
            catch
            {
                DiffCalc_LabelResult.Content = nullStr;

                ShowMboxError();
                return;
            }

            DiffCalc_LabelResult.Content = string.Format("{0:N0} روز ({1:F2} سال)", Math.Abs(calc.DateDifference.TotalDays), Math.Abs(calc.DateDifference.TotalDays / 365));
        }


        // --- Date Addition/Subtraction Calculator ---

        private void AddCalc_Calculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                calc.AddToDate
                (
                    year: Convert.ToInt32(AddCalc_TBoxYear.Text), month: Convert.ToInt32(AddCalc_CBoxMonth.Text), day: Convert.ToInt32(AddCalc_CBoxDay.Text), cal: GetCalendar(AddCalc_CBoxConFromType.SelectedIndex),

                    addYears: Math.Abs(Convert.ToInt32(AddCalc_TBoxAddYear.Text)), addMonths: Math.Abs(Convert.ToInt32(AddCalc_CBoxAddMonth.Text)), addDays: Math.Abs(Convert.ToInt32(AddCalc_CBoxAddDay.Text)),
                    toAdd: AddCalc_RBtnAdd.IsChecked == true
                );
            }
            catch
            {
                AddCalc_LabelResult.Content = nullStr;

                ShowMboxError();
                return;
            }

            AddCalc_LabelResult.Content = calc.ResultAdd;
        }

        private void AddCalc_CopyDate_Click(object sender, RoutedEventArgs e)
        {
            CopyToClipboard(AddCalc_LabelResult.Content.ToString());
        }


        // --- About Tab ---

        private void About_Link_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Wirmaple73");
        }


        // --- Menu ---

        private void Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        // --- Miscellaneous ---

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FillCBoxValues();
        }

        private void FillCBoxValues()
        {
            for (byte i = 1; i <= 31; i++)  // Days
            {
                ConCalc_CBoxDay.Items.Add(i);

                DiffCalc_CBoxDay1.Items.Add(i);
                DiffCalc_CBoxDay2.Items.Add(i);

                AddCalc_CBoxDay.Items.Add(i);
            }

            for (byte i = 1; i <= 12; i++)  // Months
            {
                ConCalc_CBoxMonth.Items.Add(i);

                DiffCalc_CBoxMonth1.Items.Add(i);
                DiffCalc_CBoxMonth2.Items.Add(i);

                AddCalc_CBoxMonth.Items.Add(i);
            }
        }

        private Calendar GetCalendar(int selIndex)
        {
            switch (selIndex)
            {
                case 0:  // Persian
                    return new PersianCalendar();

                case 1:  // Hijri
                    return new UmAlQuraCalendar();

                case 2:  // Gregorian
                    return new GregorianCalendar();

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ShowMboxError()
        {
            MessageBox.Show(".لطفا یک تاریخ معتبر را وارد کنید", "خطا", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.RightAlign);
        }

        private void CopyToClipboard(string text)
        {
            Clipboard.SetText(text);
        }
    }

    class DCalculator
    {
        PersianCalendar pc = new PersianCalendar();
        UmAlQuraCalendar qc = new UmAlQuraCalendar();  // Seems to be more accurate than HijriCalendar (?)
        GregorianCalendar gc = new GregorianCalendar();

        DateTime dt, dt2;
        TimeSpan ts;

        string resultPc, resultQc, resultGc, resultAdd;

        public string ResultPC
        {
            get { return resultPc; }
        }

        public string ResultQc
        {
            get { return resultQc; }
        }

        public string ResultGC
        {
            get { return resultGc; }
        }

        public string ResultAdd
        {
            get { return resultAdd; }
        }

        public TimeSpan DateDifference
        {
            get { return ts; }
        }

        public void ConvertDate(int year, int month, int day, Calendar cal)
        {
            UpdateDateTime(year, month, day, cal);
            GetNewDates();
        }

        public void SetTodayDate()
        {
            dt = DateTime.Now;
            GetNewDates();
        }

        public void CalculateDifference(int year, int month, int day, int year2, int month2, int day2, Calendar cal)
        {
            UpdateDateTime(year, month, day, cal);
            dt2 = new DateTime(year2, month2, day2, cal);

            ts = dt.Subtract(dt2);
        }

        public void AddToDate(int year, int month, int day, Calendar cal, int addYears, int addMonths, int addDays, bool toAdd)
        {
            UpdateDateTime(year, month, day, cal);

            if (toAdd)
                AddToDate(addYears, addMonths, addDays);  // Add to date
            else
                AddToDate(-addYears, -addMonths, -addDays);  // Subtract from date


            resultAdd = string.Format("{0}/{1}/{2}", cal.GetYear(dt), cal.GetMonth(dt), cal.GetDayOfMonth(dt));
        }

        private void AddToDate(int years, int months, int days)
        {
            dt = dt.AddYears(years);
            dt = dt.AddMonths(months);
            dt = dt.AddDays(days);
        }

        private void UpdateDateTime(int year, int month, int day, Calendar cal)
        {
            dt = new DateTime(year, month, day, cal);
        }

        private void GetNewDates()
        {
            resultPc = string.Format("{0}/{1}/{2}", pc.GetYear(dt), pc.GetMonth(dt), pc.GetDayOfMonth(dt));
            resultQc = string.Format("{0}/{1}/{2}", qc.GetYear(dt), qc.GetMonth(dt), qc.GetDayOfMonth(dt));
            resultGc = string.Format("{0}/{1}/{2}", gc.GetYear(dt), gc.GetMonth(dt), gc.GetDayOfMonth(dt));
        }
    }
}