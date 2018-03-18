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
using System.Threading;
using System.Collections.ObjectModel;

namespace MiningImitator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class VideoCard
    {
        public string Name { get; set; }
        public int Power { get; set; }
        public Thread thr { get; set; }
        public string State { get; set; }
    }

    public partial class MainWindow : Window
    {
        public double BTC = 0;
        public double ETH = 0;
        public double KRB = 0;
        public object Bobj = new object();
        public object Eobj = new object();
        public object Kobj = new object();
        public ObservableCollection<VideoCard> Cards { get; set; }
        public void BTCplus(object a)
        {
            VideoCard vc = a as VideoCard;
            MessageBox.Show(vc.thr.ThreadState.ToString());
            for (; ; )
            {
                lock (Bobj)
                {
                    BTC++;
                    this.Dispatcher.Invoke(() =>
                    {
                        BTCLabel.Content = BTC.ToString();
                    }
                    );
                }
                Thread.Sleep(1000 - vc.Power);
            }
        }
        public void ETHplus(object a)
        {
            VideoCard vc = a as VideoCard;
            for (; ; )
            {
                lock (Eobj)
                {
                    ETH++;
                    this.Dispatcher.Invoke(() =>
                    {
                        ETHLabel.Content = ETH.ToString();
                    }
                        );
                }
                Thread.Sleep(1000 - vc.Power);
            }
        }
        public void KRBplus(object a)
        {
            VideoCard vc = a as VideoCard;
            for (; ; )
            {
                lock (Kobj)
                {
                    KRB++;
                    this.Dispatcher.Invoke(() =>
                    {
                        KRBLabel.Content = KRB.ToString();
                    }
                        );
                }
                Thread.Sleep(1000 - vc.Power);
            }
        }
        public MainWindow()
        {
            InitializeComponent();

            Cards = new ObservableCollection<VideoCard>() { new VideoCard() {
                Name = "MSI GTX 1060", Power = 600    } ,
                new VideoCard() { Name = "MSI GTX 1080 Ti", Power = 850  },
                new VideoCard() { Name = "Gigabyte GTX 1080 Ti", Power=1000 },
                new VideoCard() { Name = "INNO3D GTX 1080 Ti", Power = 800 },
                new VideoCard() { Name = "Asus PCI-Ex GeForce GT", Power = 250  }
            };
            MyList.ItemsSource = Cards;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            VideoCard card = MyList.SelectedItem as VideoCard;

            switch ((CardCurrency.SelectedItem as ComboBoxItem).Content.ToString())
            {
                case "BTC": { card.thr = new Thread(BTCplus); break; }
                case "KRB": { card.thr = new Thread(KRBplus); break; }
                case "ETH": { card.thr = new Thread(ETHplus); break; }
                default: { return; }
            }
            card.thr.Start(card);
            card.State = "Running";
            Start.IsEnabled = false;
            Stop.IsEnabled = true;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            VideoCard card = MyList.SelectedValue as VideoCard;
            card.thr.Abort();
            card.State = "Suspended";
            Start.IsEnabled = true;
            Stop.IsEnabled = false;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (VideoCard v in Cards)
            {
                if (v.thr != null)
                    v.thr.Abort();
            }
            Application.Current.Shutdown();
        }
        private void MyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VideoCard vc = MyList.SelectedValue as VideoCard;
            if (vc.thr == null)
            {
                Start.IsEnabled = true;
                Stop.IsEnabled = false;
                return;
            }
            MessageBox.Show(vc.State);
            if (vc.State == "Running")
            {
                Start.IsEnabled = false;
                Stop.IsEnabled = true;
            }
            if (vc.thr.ThreadState.ToString() == "Suspended")
            {
                Start.IsEnabled = true;
                Stop.IsEnabled = false;
            }
        }
    }
}
