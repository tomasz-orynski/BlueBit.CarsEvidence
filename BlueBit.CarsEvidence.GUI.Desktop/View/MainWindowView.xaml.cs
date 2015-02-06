using BlueBit.CarsEvidence.GUI.Desktop.ViewModel;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Documents;
using BlueBit.CarsEvidence.GUI.Desktop.ViewModel.Panels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Media.Imaging;
using Xceed.Wpf.AvalonDock.Layout;

namespace BlueBit.CarsEvidence.GUI.Desktop.View
{
    public class MainWindowViewPanelSelector<T>
    {
        public T ListPanel { get; set; }
        public T EditDocument { get; set; }

        public T Select(object item)
        {
            if (item is ListPanelViewModelBase)
                return ListPanel;

            if (item is EditDocumentViewModelBase)
                return EditDocument;

            return default(T);
        }
    }

    public class MainWindowViewPanelDataTemplateSelector :
        DataTemplateSelector
    {
        private readonly MainWindowViewPanelSelector<DataTemplate> _selector = new MainWindowViewPanelSelector<DataTemplate>();

        public DataTemplate ListPanel { get { return _selector.ListPanel; } set { _selector.ListPanel = value; } }
        public DataTemplate EditDocument { get { return _selector.EditDocument; } set { _selector.EditDocument = value; } }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return _selector.Select(item)
                ?? base.SelectTemplate(item, container);
        }
    }

    public class MainWindowViewPanelStyleSelector :
        StyleSelector
    {
        private readonly MainWindowViewPanelSelector<Style> _selector = new MainWindowViewPanelSelector<Style>();

        public Style ListPanel { get { return _selector.ListPanel; } set { _selector.ListPanel = value; } }
        public Style EditDocument { get { return _selector.EditDocument; } set { _selector.EditDocument = value; } }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            return _selector.Select(item)
                ?? base.SelectStyle(item, container);
        }
    }

    public class MainWindowViewLayoutUpdateStrategy : 
        ILayoutUpdateStrategy
    {
        public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
        {
            var panel = anchorableToShow.Content as IPanelViewModel;
            if (panel == null) return false;
            var name = panel.Identifier.ToString();
            var pane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(d => d.Name == name);
            if (pane == null) return false;
            pane.Children.Add(anchorableToShow);
            return true;
        }

        public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
        {
        }

        public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
        {
            return false;
        }

        public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
        {
        }
    }


    public partial class MainWindowView : RibbonWindow
    {
        public MainWindowView()
        {
            Configuration.Settings.Init();
            InitializeComponent();

            Width = System.Windows.SystemParameters.PrimaryScreenWidth * 0.8;
            Height = System.Windows.SystemParameters.PrimaryScreenHeight * 0.8;
            Left = (System.Windows.SystemParameters.PrimaryScreenWidth - Width) * 0.5;
            Top = (System.Windows.SystemParameters.PrimaryScreenHeight - Height) * 0.5;

#if DEBUG
            var secondaryScreen = System.Windows.Forms.Screen.AllScreens.FirstOrDefault(_ => !_.Primary);
            if (secondaryScreen != null)
            {
                var dx = 1d;
                var dy = 0.6d;
                var area = secondaryScreen.WorkingArea;
                Width = area.Width * dx;
                Height = area.Height * dy;
                Left = area.Left + (area.Width - Width) * 0.5;
                Top = area.Top + (area.Height - Height) * 0.5;
            }
#endif
        }
    }
}
