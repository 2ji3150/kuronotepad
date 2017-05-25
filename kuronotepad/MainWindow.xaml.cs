using Hnx8.ReadJEnc;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace kuronotepad {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            DataContext = vm;
            PreviewMouseWheel += Zoom_MouseWheel;
            textBox.TextArea.Caret.PositionChanged += Caret_PositionChanged;
        }
        public static RoutedCommand GetTime = new RoutedCommand();
        ViewModel vm = new ViewModel();
        CharCode c;
        public string editpath = string.Empty;
        string originaltext = string.Empty;
        bool Dirtyflag() => vm.IsDirty && textBox.Text != originaltext;

        private void Zoom_MouseWheel(object sender, MouseWheelEventArgs e) {
            if ((Keyboard.Modifiers & ModifierKeys.Control) <= 0) return;
            float delta;
            if (e.Delta > 0) delta = 10f;
            else {
                if (textBox.FontSize <= 10) return;
                delta = -10f;
            }
            textBox.FontSize += delta;
        }

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) {
            #region  編集中のテキストを破棄するどうかを確認
            if ((editpath == string.Empty) && (textBox.Text != string.Empty)) {
                MessageBoxResult result = MessageBox.Show("無題 への変更内容を保存しますか?", "クロノメモ帳", MessageBoxButton.YesNoCancel);
                switch (result) {
                    case MessageBoxResult.Yes:
                        SaveAsMenuItem.Command.Execute(null);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            else if (Dirtyflag()) {
                MessageBoxResult result = MessageBox.Show(editpath + " へ" + Environment.NewLine + "の変更内容を保存しますか?", "クロノメモ帳", MessageBoxButton.YesNoCancel);
                switch (result) {
                    case MessageBoxResult.Yes:
                        SaveMenuItem.Command.Execute(null);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            #endregion
            OpenFileDialog odlg = new OpenFileDialog() {
                Filter = "テキスト文書 (*.txt)|*.txt|すべてのファイル (*.*)|*.*"
            };
            if (odlg.ShowDialog() == false) return;
            //編集中のパスを取得
            editpath = odlg.FileName;
            LoadText();
        }

        private void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) {
            #region 保存ダイアログ
            SaveFileDialog sdlg = new SaveFileDialog() {
                Filter = "テキスト文書 (*.txt)|*.txt|すべてのファイル (*.*)|*.*",
                FileName = Path.GetFileName(editpath)
            };
            if (sdlg.ShowDialog() == false) return;
            editpath = sdlg.FileName;
            #endregion
            #region Save
            string edittext = textBox.Text;
            if (vm.UTF8) SaveText(edittext, Encoding.UTF8);
            else {
                Encoding sjisEnc = Encoding.GetEncoding(932);
                string sjis_edittext = sjisEnc.GetString(sjisEnc.GetBytes(edittext));
                if (sjis_edittext == edittext) {
                    SaveText(edittext, sjisEnc);
                }
                else {
                    MessageBoxResult result = MessageBox.Show(editpath + Environment.NewLine + "このファイルはUnicode形式の文字を含んでいます｡" + Environment.NewLine + "保存形式を[UTF8]に切り替えますか?", "クロノメモ帳", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (result == MessageBoxResult.Yes) {
                        vm.UTF8 = true;
                        SaveText(edittext, Encoding.UTF8);
                        vm.Encode = "UTF-8";
                    }
                    else {
                        SaveText(edittext, sjisEnc);
                        textBox.Text = sjis_edittext;
                    }
                }
            }
            #endregion            
        }

        private void NewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) {
            #region  編集中のテキストを破棄するどうかを確認
            if ((editpath == string.Empty) && (textBox.Text != string.Empty)) {
                MessageBoxResult result = MessageBox.Show("無題 への変更内容を保存しますか?", "クロノメモ帳", MessageBoxButton.YesNoCancel);
                switch (result) {
                    case MessageBoxResult.Yes:
                        SaveAsMenuItem.Command.Execute(null);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            else if (Dirtyflag()) {
                MessageBoxResult result = MessageBox.Show(editpath + " へ" + Environment.NewLine + "の変更内容を保存しますか?", "クロノメモ帳", MessageBoxButton.YesNoCancel);
                switch (result) {
                    case MessageBoxResult.Yes:
                        SaveMenuItem.Command.Execute(null);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            #endregion
            editpath = string.Empty;
            vm.Title = "無題 - クロノメモ帳";
            textBox.Text = string.Empty;
            vm.IsDirty = false;
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) {
            if (editpath == string.Empty) {
                SaveAsMenuItem.Command.Execute(null);
            }
            else {
                #region Save
                string edittext = textBox.Text;
                if (vm.UTF8) SaveText(edittext, Encoding.UTF8);
                else {
                    Encoding sjisEnc = Encoding.GetEncoding(932);
                    string sjis_edittext = sjisEnc.GetString(sjisEnc.GetBytes(edittext));
                    if (sjis_edittext == edittext) {
                        SaveText(edittext, sjisEnc);
                    }
                    else {
                        MessageBoxResult result = MessageBox.Show(editpath + Environment.NewLine + "このファイルはUnicode形式の文字を含んでいます｡" + Environment.NewLine + "保存形式を[UTF8]に切り替えますか?", "クロノメモ帳", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        if (result == MessageBoxResult.Yes) {
                            vm.UTF8 = true;
                            SaveText(edittext, Encoding.UTF8);
                            vm.Encode = "UTF-8";
                        }
                        else {
                            SaveText(edittext, sjisEnc);
                            textBox.Text = sjis_edittext;
                        }
                    }
                }
                #endregion
            }
        }

        private void SaveText(string contents, Encoding encode) {
            try {
                //テキストボックスの内容をファイルに書き込み
                File.WriteAllText(editpath, contents, encode);
                //タイトルを更新
                vm.Title = Path.GetFileName(editpath);
                //変更確認用初期状態テキストを更新
                originaltext = contents;
                vm.IsDirty = false;
            }
            catch (UnauthorizedAccessException ex) {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                SaveAsMenuItem.Command.Execute(null);
            }
        }

        public void LoadText() {   //Windowのタイトルをファイル名に変更
            vm.Title = Path.GetFileName(editpath);
            try {
                string text;
                FileInfo file = new FileInfo(editpath);

                using (FileReader reader = new FileReader(file)) {
                    //判別読み出し実行。判別結果はReadメソッドの戻り値で把握できます
                    c = reader.Read(file);
                    //実際に読み出したテキストは、Textプロパティから取得できます
                    //（非テキストファイルの場合は、nullが設定されます）
                    text = reader.Text;
                }
                if (text == null) {
                    byte[] textbyte = File.ReadAllBytes(editpath);
                    c = ReadJEnc.CN.GetEncoding(textbyte, textbyte.Length, out text);
                    if (c == null) {
                        c = ReadJEnc.KR.GetEncoding(textbyte, textbyte.Length, out text);
                        if (c == null) {
                            c = ReadJEnc.TW.GetEncoding(textbyte, textbyte.Length, out text);
                            if (c == null) {
                                MessageBox.Show("サポートされていないエンコードもしくはバイナリです");
                            }
                        }
                    }
                }
                //最後
                textBox.Text = originaltext = text;
                vm.IsDirty = false;
                vm.Encode = c.Name;
                vm.UTF8 = vm.Encode == "UTF-8";
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Caret_PositionChanged(object sender, EventArgs e) => Task.Run(() => {
            Dispatcher.BeginInvoke((Action)(() => {
                ICSharpCode.AvalonEdit.TextViewPosition tp = textBox.TextArea.Caret.Position;
                vm.CaretPosition = $"{tp.Line} 行    {tp.Column} 列";
            }));
        });

        private void GetTime_Executed(object sender, ExecutedRoutedEventArgs e) {
            textBox.SelectedText = DateTime.Now.ToString();
            textBox.SelectionLength = 0;
            textBox.TextArea.Caret.Offset += DateTime.Now.ToString().Length;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            #region  編集中のテキストを破棄するどうかを確認
            if ((editpath == string.Empty) && (textBox.Text != string.Empty)) {
                MessageBoxResult result = MessageBox.Show("無題 への変更内容を保存しますか?", "クロノメモ帳", MessageBoxButton.YesNoCancel);
                switch (result) {
                    case MessageBoxResult.Yes:
                        SaveAsMenuItem.Command.Execute(null);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
            else if (Dirtyflag()) {
                MessageBoxResult result = MessageBox.Show(editpath + " へ" + Environment.NewLine + "の変更内容を保存しますか?", "クロノメモ帳", MessageBoxButton.YesNoCancel);
                switch (result) {
                    case MessageBoxResult.Yes:
                        SaveMenuItem.Command.Execute(null);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
            #endregion
        }
    }
}
