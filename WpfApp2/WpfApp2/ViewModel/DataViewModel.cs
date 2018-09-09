using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using WpfApp2.Model;

namespace WpfApp2.ViewModel
{
    public class DataViewModel : INotifyPropertyChanged
    {
        #region fields
        private DataModel obj = new DataModel();
        public event PropertyChangedEventHandler PropertyChanged;
        private bool visibilityControl = false;
        private static string hMessage = "";
        private int index1Start;
        private int index1End;
        private int index2Start;
        private int index2End;
        private bool merged;
        private int currentLine = 0;
        private bool first = true;
        private bool second = true;
        private string tb1Copy = "";
        private string tb2Copy = "";
        private bool conflict = false;
        private static object syncLock = new object();
        private List<int> confilctIndex = new List<int>();
        private string textSearch = "";
        private List<Thread> threadIds = new List<Thread>();
        private List<int> positions = new List<int>();
        private RichTextBox tmp;
        private static object tmpLock = new object();
        #endregion

        #region Properties
        public string TextBox1
        {
            get { return obj.Text1; }
            set
            {
                obj.Text1 = value;
                RaisePropertyChanged("TextBox1");
            }
        }

        public string TextBox2
        {
            get { return obj.Text2; }
            set
            {
                obj.Text2 = value;
                RaisePropertyChanged("TextBox2");
            }
        }

        public string TextBox3
        {
            get { return obj.Text3; }
            set
            {
                obj.Text3 = value;
                RaisePropertyChanged("TextBox3");
            }
        }

        public bool VisibilityValue
        {
            get { return visibilityControl; }
            set
            {
                visibilityControl = value;
                RaisePropertyChanged("VisibilityControl");
            }
        }

        public bool MergedValue
        {
            get { return merged; }
            set
            {
                merged = value;
                RaisePropertyChanged("MergeControl");
            }
        }

        public string Hmessage
        {
            get { return hMessage; }
            set
            {
                hMessage = value;
                RaisePropertyChanged("Hmessage");
            }
        }

        public int Index1Start
        {
            get { return index1Start; }
            set { index1Start = value; }
        }

        public int Index1End
        {
            get { return index1End; }
            set { index1End = value; }
        }

        public int Index2Start
        {
            get { return index2Start; }
            set { index2Start = value; }
        }

        public int Index2End
        {
            get { return index2End; }
            set { index2End = value; }
        }

        public Visibility MergeControl
        {
            get { return MergedValue ? Visibility.Visible : Visibility.Hidden; }
        }

        public Visibility VisibilityControl
        {
            get { return visibilityControl ? Visibility.Visible : Visibility.Hidden; }
        }

        public string TextForSerch
        {
            get { return textSearch; }
            set
            {
                if (textSearch == value)
                    return;

                textSearch = value;
            }
        }

        public List<int> Positions
        {
            get { return positions; }
            private set
            {
                positions = value;
                ColorSearchedText();
            }
        }
        #endregion

        #region Commands
        public ICommand LoadTB1
        {
            get { return new DelegateCommand<object>(loadtextBox1); }
        }

        public ICommand LoadTB2
        {
            get { return new DelegateCommand<object>(loadTextBox2); }
        }

        public ICommand MergeCommand
        {
            get { return new DelegateCommand(merge); }
        }

        public ICommand MergeLeftCommand
        {
            get { return new DelegateCommand(mergeLeft); }
        }

        public ICommand MergeRightCommand
        {
            get { return new DelegateCommand(mergeRight); }
        }

        public ICommand SaveCommand
        {
            get { return new DelegateCommand(Save); }
        }

        public ICommand ClearTB3
        {
            get { return new DelegateCommand(ClearTextBox3); }
        }

        public ICommand MergeAllFromLeftCommand
        {
            get { return new DelegateCommand(AllFromLeft); }
        }

        public ICommand MergeAllFromRightCommand
        {
            get { return new DelegateCommand(AllFromRight); }
        }

        public ICommand SearchCommand
        {
            get { return new DelegateCommand<object>(Search); }
        }
        #endregion

        #region Methods
        private void loadtextBox1(object document)
        {
            OpenFileDialog diag = new OpenFileDialog();
            if ((bool)diag.ShowDialog())
            {
                string filename = diag.FileName;
                StringBuilder sb = new StringBuilder();
                string s1 = "";
                StreamReader sr = new StreamReader(filename);
                {

                    while ((s1 = sr.ReadLine()) != null)
                        sb.AppendLine(s1);
                }
                string tmp = sb.ToString();
                Run r = new Run();
                Paragraph p = new Paragraph(r);

                Binding b = new Binding();
                b.Source = this;
                b.Path = new PropertyPath("TextBox1");
                b.Mode = BindingMode.OneWay;
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                r.SetBinding(Run.TextProperty, b);

                ((RichTextBox)document).Document.Blocks.Clear();
                ((RichTextBox)document).Document.Blocks.Add(p);

                TextBox1 = tmp;
                visibilityControl = false;
                first = true;
                MergedValue = true;
                confilctIndex.Clear();
                currentLine = 0;
                lock (syncLock)
                {
                    Hmessage = "";
                }
            }
        }

        private void loadTextBox2(object document)
        {
            OpenFileDialog diag = new OpenFileDialog();

            if ((bool)diag.ShowDialog())
            {
                string filename = diag.FileName;
                StringBuilder sb = new StringBuilder();
                string s1 = "";

                StreamReader sr = new StreamReader(filename);
                {

                    while ((s1 = sr.ReadLine()) != null)
                        sb.AppendLine(s1);
                }
                string tmp = sb.ToString();
                Run r = new Run();
                Paragraph p = new Paragraph(r);

                Binding b = new Binding();
                b.Source = this;
                b.Path = new PropertyPath("TextBox2");
                b.Mode = BindingMode.OneWay;
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                r.SetBinding(Run.TextProperty, b);

                ((RichTextBox)document).Document.Blocks.Clear();
                ((RichTextBox)document).Document.Blocks.Add(p);

                TextBox2 = tmp;
                visibilityControl = false;
                MergedValue = true;
                second = true;
                confilctIndex.Clear();
                currentLine = 0;
                lock (syncLock)
                {
                    Hmessage = "";
                }
            }
        }

        private void merge()
        {
            if (currentLine == 0 && first == true)
            {
                tb1Copy = TextBox1;
                first = false;
            }

            if (currentLine == 0 && second == true)
            {
                tb2Copy = TextBox2;
                second = false;
            }

            string[] s1 = tb1Copy.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            string[] s2 = tb2Copy.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            string s3 = TextBox3;

            int count = s1.Count() >= s2.Count() ? s2.Count() : s1.Count();

            if (currentLine > count)
            {
                currentLine = 0;
            }

            while (currentLine < count - 1)
            {
                if (s1[currentLine] == s2[currentLine])
                {
                    s3 += s1[currentLine] + "\r\n";
                    currentLine++;
                }
                else
                {
                    string param1 = s1[currentLine];
                    string param2 = s2[currentLine];
                    int param3 = currentLine;
                    conflict = true;
                    lock (syncLock)
                    {
                        confilctIndex.Add(currentLine);//za realizaciju ispisivanja tredova Hammingove Distance
                        //Thread t = new Thread(() => CalculateHumber(s1[currentLine], s2[currentLine], currentLine));
                        //t.Start();
                        Task a = Task.Run(() => CalculateHumber(param1, param2, param3));
                    }
                    Index1Start = 0;
                    Index2Start = 0;
                    for (int i = 0; i < currentLine; i++)
                    {
                        Index1Start += (s1[i].Length + 2);
                        Index2Start += (s2[i].Length + 2);
                    }

                    Index1End = Index1Start + s1[currentLine].Length;
                    Index2End = Index2Start + s2[currentLine].Length;
                    break;
                }
            }
            if (!conflict)
            {
                if (count < s1.Count())
                {
                    count = count - 1;
                    for (int i = count; i < s1.Count() - 1; i++)
                    {
                        s3 += s1[i] + "\r\n";
                    }

                    MergedValue = false;
                    first = false;
                    second = false;
                    TextBox3 = s3;
                }
                else if (count < s2.Count())
                {
                    for (int i = count - 1; i < s2.Count() - 1; i++)
                    {
                        s3 += s2[count] + "\r\n";
                    }

                    MergedValue = false;
                    first = false;
                    second = false;
                    TextBox3 = s3;
                }
            }
            TextBox3 = s3;
            VisibilityValue = conflict;
        }

        private void mergeLeft()
        {
            string[] separators = new string[] { "\r\n" };

            string[] s1 = tb1Copy.Split(separators, StringSplitOptions.None);
            string[] s2 = tb2Copy.Split(separators, StringSplitOptions.None);
            string s3 = TextBox3;

            if (conflict)
            {
                s3 += s1[currentLine] + "\r\n";
                currentLine++;
                conflict = false;
            }

            TextBox3 = s3;
            VisibilityValue = conflict;
            merge();
        }

        private void mergeRight()
        {
            string[] separators = new string[] { "\r\n" };

            string[] s1 = tb1Copy.Split(separators, StringSplitOptions.None);
            string[] s2 = tb2Copy.Split(separators, StringSplitOptions.None);
            string s3 = TextBox3;

            if (conflict)
            {
                s3 += s2[currentLine] + "\r\n";
                currentLine++;
                conflict = false;
            }

            TextBox3 = s3;
            VisibilityValue = conflict;
            merge();
        }

        private void AllFromLeft()
        {
            string[] s1;
            string[] s2;

            if (currentLine == 0 && first == true)
            {
                tb1Copy = TextBox1;
            }

            if (currentLine == 0 && first == true)
            {
                tb2Copy = TextBox2;
            }

            s1 = tb1Copy.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            s2 = tb2Copy.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            int count = s1.Count() >= s2.Count() ? s2.Count() : s1.Count();
            string s3 = TextBox3;

            while (currentLine < s1.Count())
            {
                s3 += s1[currentLine] + "\r\n";
                currentLine++;
            }
            int i = 0;
            while (i < count - 1)
            {
                int param3 = i;
                string param1 = s1[i];
                string param2 = s2[i];
                if (s1[i] != s2[i])
                {
                    if (!confilctIndex.Contains(i))
                    {
                        lock (syncLock)
                        {
                            confilctIndex.Add(i);
                            //Thread t = new Thread(() => CalculateHumber(param1, param2, param3));
                            //t.Start();
                            Task a = Task.Run(() => CalculateHumber(param1, param2, param3));
                        }
                    }
                }
                i++;
            }

            first = false;
            second = false;
            VisibilityValue = false;
            TextBox3 = s3;
            MergedValue = false;
        }

        private void AllFromRight()
        {
            string[] s2;
            string[] s1;
            if (currentLine == 0 && second == true)
            {
                tb2Copy = TextBox2;
            }

            if (currentLine == 0 && first == true)
            {
                tb1Copy = TextBox1;
            }

            s2 = tb2Copy.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            s1 = tb1Copy.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            int count = s1.Count() >= s2.Count() ? s2.Count() : s1.Count();
            string s3 = TextBox3;

            while (currentLine < s2.Count())
            {
                s3 += s2[currentLine] + "\r\n";
                currentLine++;
            }

            int i = 0;
            while (i < count - 1)
            {
                int param3 = i;
                string param1 = s1[i];
                string param2 = s2[i];
                if (s1[i] != s2[i])
                {
                    if (!confilctIndex.Contains(i))
                    {
                        lock (syncLock)
                        {
                            confilctIndex.Add(i);
                            //Thread t = new Thread(() => CalculateHumber(param1, param2, param3));
                            //t.Start();
                            Task a = Task.Run(() => CalculateHumber(param1, param2, param3));
                        }
                    }
                }
                i++;
            }

            second = false;
            first = false;
            VisibilityValue = false;
            TextBox3 = s3;
            MergedValue = false;
        }

        private void Save()
        {
            SaveFileDialog diag = new SaveFileDialog();
            diag.DefaultExt = "txt";

            if (diag.ShowDialog() == true)
            {
                using (StreamWriter sw = new StreamWriter(diag.FileName))
                {
                    sw.WriteLine(TextBox3);
                }
            }
        }

        private void ClearTextBox3()
        {
            TextBox3 = "";
            MergedValue = true;
            currentLine = 0;
            conflict = false;
            Hmessage = "";
            first = false;
            second = false;
            confilctIndex.Clear();
            if (tmp != null)
            {
                tmp.SelectAll();
                tmp.Selection.Text = "";
                Run r = new Run();
                Paragraph p = new Paragraph(r);

                Binding b = new Binding();
                b.Source = this;
                b.Path = new PropertyPath("TextBox3");
                b.Mode = BindingMode.OneWay;
                b.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                r.SetBinding(Run.TextProperty, b);

                tmp.Document.Blocks.Clear();
                tmp.Document.Blocks.Add(p);
            }
        }

        private void CalculateHumber(string s1, string s2, int currentLine)
        {
            int curr = currentLine;
            Random r = new Random();
            bool executed = false;
            int r2 = r.Next() % 10000;
            int previous = -1;

            Thread.Sleep(r2);
            int count = s1.Length > s2.Length ? s2.Length : s1.Length;

            int HNumber = 0;

            for (int i = 0; i < count; i++)
            {
                if (s1.ElementAt(i) != s2.ElementAt(i))
                {
                    HNumber += 1;
                }
            }

            if (s1.Length > count)
            {
                HNumber += s1.Length - count;
            }
            else if (s2.Length > count)
            {
                HNumber += s2.Length - count;
            }

            if (Hmessage == "" && curr == confilctIndex[0])
            {
                Hmessage += " Hamming distance on line" + (currentLine + 1).ToString() + " is " + HNumber.ToString() + "\n";
            }
            else
            {
                lock (syncLock)
                {
                    for (int i = 0; i < confilctIndex.Count; i++)
                    {
                        if (confilctIndex[i] == curr)
                        {
                            previous = confilctIndex[i - 1];
                        }
                    }
                }
                while (!executed)
                {
                    Thread.Sleep(100);
                    lock (syncLock)
                    {
                        if (Hmessage.Contains("line" + (previous + 1).ToString()))
                        {
                            Hmessage += " Hamming distance on line" + (currentLine + 1).ToString() + " is " + HNumber.ToString() + "\n";
                            executed = false;
                            break;
                        }
                    }
                }
            }

        }

        private async Task<int> TaskDelay()
        {
            await Task.Delay(700);
            return 1;
        }

        private async void Search(object textBox)
        {
            tmp = (RichTextBox)textBox;

            Task<int> resultTask = TaskDelay();
            int result = await resultTask;
            string paramText = TextForSerch;
            lock (tmpLock)
            {
                tmp = (RichTextBox)textBox;
            }

            if (paramText != "")
            {
                lock (tmpLock)
                {
                    tmp.SelectAll();
                    tmp.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, SystemColors.WindowBrush);
                }
                Thread t = new Thread(() => SearchtextTread(paramText, textBox));
                t.Name = paramText;

                if (threadIds.Count > 0)
                {
                    for (int i = 0; i < threadIds.Count; i++)
                    {
                        threadIds.ElementAt(i).Abort();
                    }

                    for (int i = 0; i < threadIds.Count; i++)
                    {
                        threadIds.RemoveAt(i);
                    }

                }
                threadIds.Add(t);
                t.Start();

            }
            else
            {
                if (threadIds.Count > 0)
                {
                    for (int i = 0; i < threadIds.Count - 1; i++)
                    {
                        threadIds.ElementAt(i).Abort();
                    }

                    for (int i = 0; i < threadIds.Count - 1; i++)
                    {
                        threadIds.RemoveAt(i);
                    }

                }
                lock (tmpLock)
                {
                    tmp.SelectAll();
                    tmp.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, SystemColors.WindowBrush);
                }
            }


        }

        private void SearchFromProperty()
        {
            string paramText = TextForSerch;
            if (paramText != "")
            {
                lock (tmpLock)
                {
                    tmp.SelectAll();
                    tmp.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, SystemColors.WindowBrush);
                }
                Thread t = new Thread(() => SearchtextTread(paramText, tmp));

                t.Name = paramText;
                threadIds.Add(t);

                if (threadIds.Count > 0)
                {
                    for (int i = 0; i < threadIds.Count - 1; i++)
                    {
                        threadIds.ElementAt(i).Abort();
                    }

                    for (int i = 0; i < threadIds.Count - 1; i++)
                    {
                        threadIds.RemoveAt(i);
                    }

                }

                t.Start();

            }
            else
            {
                if (threadIds.Count > 0)
                {
                    for (int i = 0; i < threadIds.Count - 1; i++)
                    {
                        threadIds.ElementAt(i).Abort();
                    }

                    for (int i = 0; i < threadIds.Count - 1; i++)
                    {
                        threadIds.RemoveAt(i);
                    }

                }

                tmp.SelectAll();
                tmp.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, SystemColors.WindowBrush);
            }
        }

        private void SearchtextTread(string txt, object textBox)
        {
            if (txt != "")
            {
                Thread.Sleep(1200);
                List<int> pos = null;
                int length;
                pos = new List<int>();

                lock (tmpLock)
                {
                    RichTextBox tmp = (RichTextBox)textBox;
                    length = tmp.Selection.Text.Length;
                }


                for (int i = 0; i < length - txt.Length; i++)
                {
                    lock (tmpLock)
                    {
                        if (tmp.Selection.Text.Substring(i, txt.Length) == txt)
                        {
                            pos.Add(i);
                        }
                    }
                }

                if (pos != null)
                {
                    Positions = pos;
                }

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
            TextSelection textRange;
            TextPointer start;
            lock (tmpLock)
            {
                // Get text selection:
                 textRange = rtb.Selection;

                // Get text starting point:
                 start = rtb.Document.ContentStart;
            }

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

        public void ColorSearchedText()
        {
            if (Positions != null)
            {
                foreach (int position in positions)
                {
                    tmp.Dispatcher.Invoke(() => Select(tmp, position, TextForSerch.Length, Colors.Yellow));
                }
            }
        }

        private void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
        #endregion
    }
}
