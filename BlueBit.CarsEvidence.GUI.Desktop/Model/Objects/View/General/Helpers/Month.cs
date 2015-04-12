using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace BlueBit.CarsEvidence.GUI.Desktop.Model.Objects.View.General.Helpers
{
    public class Month :
        IViewGeneralObject
    {
        public byte Number { get; set; }
        public string Name { get { return CultureInfo.CurrentUICulture.DateTimeFormat.AbbreviatedMonthNames[Number-1]; } }

        public string Description { get { return string.Format("{0:D2}. {1}", Number, Name); } }
        public string DescriptionForToolTip { get { return string.Format("{0}. {1}", Number, Name); } }

        public int CompareTo(object obj) { return this.CompareDescriptionTo(obj); }
    }

    public static class MonthExtensions
    {
        private static readonly Lazy<ObservableCollection<Month>> _items = new Lazy<ObservableCollection<Month>>(
            () => new ObservableCollection<Month>(
                Enumerable
                    .Range(1,12)
                    .Select(_ => new Month() { Number = (byte)_ }))
            );

        public static ObservableCollection<Month> Items { get { return _items.Value; } }

        public static Month GetMonth(this byte month) { return Items.SingleOrDefault(_ => _.Number == month); }
    }
}
