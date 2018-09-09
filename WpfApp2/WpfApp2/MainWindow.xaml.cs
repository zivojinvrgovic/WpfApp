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

namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MyUserControll.ab.IsVisibleChanged += new DependencyPropertyChangedEventHandler(onVisibleShow);
           // MyUserControll.browse1.Click += new RoutedEventHandler(onClickBrowse1);
        }

        private void onVisibleShow(object sender, DependencyPropertyChangedEventArgs e)
        {

            if (MyUserControll.ab.Visibility == Visibility.Visible)
            {

                int length1 = MyViewModel.Index1End - MyViewModel.Index1Start;
                int length2 = MyViewModel.Index2End - MyViewModel.Index2Start;

                this.Select(MyUserControll.tb1, MyViewModel.Index1Start, length1, Colors.Red);
                this.Select(MyUserControll.tb2, MyViewModel.Index2Start, length2, Colors.Red);

            }
            else if (MyUserControll.ab.Visibility == Visibility.Hidden)
            {
                int length1 = MyViewModel.Index1End - MyViewModel.Index1Start;
                int length2 = MyViewModel.Index2End - MyViewModel.Index2Start;

                this.Select(MyUserControll.tb1, MyViewModel.Index1Start, length1, Colors.Green);
                this.Select(MyUserControll.tb2, MyViewModel.Index2Start, length2, Colors.Green);
            }

        }

        private static TextPointer GetTextPointAt(TextPointer from, int pos)
        {
            TextPointer ret = from;
            int i = 0;

            while ((i < pos) && (ret != null))
            {
                if ((ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text) || (ret.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None))
                    i++;

                if (ret.GetPositionAtOffset(1, LogicalDirection.Forward) == null)
                    return ret;

                ret = ret.GetPositionAtOffset(1, LogicalDirection.Forward);
            }

            return ret;
        }

        internal string Select(RichTextBox rtb, int offset, int length, Color color)
        {
            // Get text selection:
            TextSelection textRange = rtb.Selection;

            // Get text starting point:
            TextPointer start = rtb.Document.ContentStart;

            // Get begin and end requested:
            TextPointer startPos = GetTextPointAt(start, offset);
            TextPointer endPos = GetTextPointAt(start, offset + length);

            // New selection of text:
            textRange.Select(startPos, endPos);

            // Apply property to the selection:
            textRange.ApplyPropertyValue(TextElement.BackgroundProperty, new SolidColorBrush(color));

            // Return selection text:
            return rtb.Selection.Text;
        }

        private void onClickBrowse1(object sender,RoutedEventArgs e)
        {
            MyUserControll.tb1.Background = SystemColors.WindowBrush;
        }
    }
}
