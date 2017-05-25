using System.Windows;

namespace kuronotepad {
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {
            MainWindow wnd = new MainWindow();
            if (e.Args.Length == 1) {
                wnd.editpath = e.Args[0];
                wnd.LoadText();
            }
            wnd.Show();
        }
    }
}
