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
using System.Windows.Shapes;

namespace ExpressInstaller
{
    /// <summary>
    /// Логика взаимодействия для CriticalError.xaml
    /// </summary>
    public partial class CriticalError : Window
    {

        public CriticalError(string title, string description)
        {
            InitializeComponent();
            
            textBlockTitle.Text = title;
            textBlockDesc.Text = description;

            this.Closing += new System.ComponentModel.CancelEventHandler(CriticalError_Closing);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void CriticalError_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
