using System.Windows;
using System.Windows.Controls;

namespace laba4
{
    public static class ScrollIntoViewBehavior
    {
        public static readonly DependencyProperty ScrollIntoViewProperty =
            DependencyProperty.RegisterAttached("ScrollIntoView", typeof(DataGrid), typeof(ScrollIntoViewBehavior), new PropertyMetadata(null, OnScrollIntoViewPropertyChanged));



        public static DataGrid GetScrollIntoView(DependencyObject obj)
        {
            return (DataGrid)obj.GetValue(ScrollIntoViewProperty);
        }

        public static void SetScrollIntoView(DependencyObject obj, DataGrid value)
        {
            obj.SetValue(ScrollIntoViewProperty, value);
        }

        private static void OnScrollIntoViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                button.Click += (sender, args) =>
                {
                    DataGrid dataGrid = GetScrollIntoView(button);
                    if (dataGrid?.SelectedItem != null)
                    {
                        int index = dataGrid.SelectedIndex;
                        if (index > 0)
                        {
                            dataGrid.SelectedIndex = index - 1;
                            dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                        }
                    }
                };
            }
        }

        public static readonly DependencyProperty ScrollIntoViewPropertyDown =
            DependencyProperty.RegisterAttached("ScrollIntoViewDown", typeof(DataGrid), typeof(ScrollIntoViewBehavior), new PropertyMetadata(null, OnScrollIntoViewPropertyChangedDown));

        public static DataGrid GetScrollIntoViewDown(DependencyObject obj)
        {
            return (DataGrid)obj.GetValue(ScrollIntoViewPropertyDown);
        }

        public static void SetScrollIntoViewDown(DependencyObject obj, DataGrid value)
        {
            obj.SetValue(ScrollIntoViewPropertyDown, value);
        }

        private static void OnScrollIntoViewPropertyChangedDown(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                button.Click += (sender, args) =>
                {
                    DataGrid dataGrid = GetScrollIntoViewDown(button);
                    if (dataGrid?.SelectedItem != null)
                    {
                        int index = dataGrid.SelectedIndex;
                        if (index >= 0 && index < dataGrid.Items.Count - 1)
                        {
                            dataGrid.SelectedIndex = index + 1;
                            dataGrid.ScrollIntoView(dataGrid.SelectedItem);
                        }
                    }
                };
            }
        }

    }
}