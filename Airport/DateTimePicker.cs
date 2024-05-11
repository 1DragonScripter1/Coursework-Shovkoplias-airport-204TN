using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace Airport
{
    public class DateTimePicker : UserControl
    {
        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime", typeof(DateTime?), typeof(DateTimePicker), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedDateTimeChanged));

        public static readonly DependencyProperty DateTimeFormatProperty =
            DependencyProperty.Register("DateTimeFormat", typeof(string), typeof(DateTimePicker), new PropertyMetadata("dd.MM.yyyy HH:mm"));

        public static readonly DependencyProperty IsInvalidDateTimeProperty =
            DependencyProperty.Register("IsInvalidDateTime", typeof(bool), typeof(DateTimePicker), new PropertyMetadata(false));

        public DateTime? SelectedDateTime
        {
            get { return (DateTime?)GetValue(SelectedDateTimeProperty); }
            set { SetValue(SelectedDateTimeProperty, value); }
        }

        public string DateTimeFormat
        {
            get { return (string)GetValue(DateTimeFormatProperty); }
            set { SetValue(DateTimeFormatProperty, value); }
        }

        public bool IsInvalidDateTime
        {
            get { return (bool)GetValue(IsInvalidDateTimeProperty); }
            set { SetValue(IsInvalidDateTimeProperty, value); }
        }

        private TextBox DateTimeText;

        public DateTimePicker()
        {
            InitializeControl();
        }

        private static void OnSelectedDateTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DateTimePicker dateTimePicker)
            {
                dateTimePicker.UpdateText();
            }
        }

        private void InitializeControl()
        {
            DateTimeText = new TextBox();
            DateTimeText.TextChanged += DateTimeText_TextChanged;
            Content = DateTimeText;
            UpdateText();
        }

        private void DateTimeText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DateTimeText.Text))
            {
                SelectedDateTime = null;
                IsInvalidDateTime = false;
            }
            else if (DateTime.TryParseExact(DateTimeText.Text, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
            {
                SelectedDateTime = parsedDateTime;
                IsInvalidDateTime = false;
            }
            else
            {
                IsInvalidDateTime = true;
            }
        }


        private void UpdateText()
        {
            if (SelectedDateTime.HasValue)
            {
                DateTimeText.Text = SelectedDateTime.Value.ToString(DateTimeFormat);
                IsInvalidDateTime = false;
            }
            else
            {
                DateTimeText.Text = string.Empty;
                IsInvalidDateTime = true;
            }
        }
    }
}
