using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General.Helpers
{
    public class Day :
        IViewGeneralObject
    {
        private readonly DayOfWeek _dayOfWeak;

        public Tuple<int,byte> YearMonth { get; set; }
        public byte Number { get; set; }

        public string Name { get { return CultureInfo.CurrentUICulture.DateTimeFormat.DayNames[(int)_dayOfWeak]; } }
        public string Description { get { return string.Format("{0}. {1}", Number, Name); } }
        public string DescriptionForTitle { get { return string.Format("{0}. {1}", Number, Name); } }

        public bool IsWeekend { get { return _dayOfWeak == DayOfWeek.Sunday || _dayOfWeak == DayOfWeek.Saturday; } }

        public Day(Tuple<int, byte> yearMonth, byte number)
        {
            YearMonth = yearMonth;
            Number = number;
            var dt = new DateTime(yearMonth.Item1, yearMonth.Item2, number);
            _dayOfWeak = dt.DayOfWeek;
        }
    }

    public static class DayExtensions
    {
        private static readonly Dictionary<Tuple<int, byte>, ObservableCollection<Day>> _items = new Dictionary<Tuple<int, byte>, ObservableCollection<Day>>();

        public static ObservableCollection<Day> GetItems(this Tuple<int, Month> yearMonth)
        {
            Contract.Assert(yearMonth != null);
            if (yearMonth.Item2 == null)
                return new ObservableCollection<Day>();
            return Tuple.Create(yearMonth.Item1, yearMonth.Item2.Number).GetItems();
        }

        public static ObservableCollection<Day> GetItems(this Tuple<int, byte> yearMonth)
        {
            Contract.Assert(yearMonth != null);

            ObservableCollection<Day> result;
            if (!_items.TryGetValue(yearMonth, out result))
            {
                result = new ObservableCollection<Day>(
                    Enumerable
                        .Range(1, DateTime.DaysInMonth(yearMonth.Item1, yearMonth.Item2))
                        .Select(_ => new Day(yearMonth, (byte)_))
                    );
                _items.Add(yearMonth, result);
            }
            return result;
        }
    }
}
