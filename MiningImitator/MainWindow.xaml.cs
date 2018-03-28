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
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace MiningImitator
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class VideoCard : INotifyPropertyChanged
    {
         
        public string Name { get; set; }
        public int Power { get; set; }
        public Thread thr { get; set; }
        public string State { get; set; }
         public int current_session_mined;
        public event PropertyChangedEventHandler PropertyChanged;
        public int CurrentSessionMined
        {
            get
            {
                return current_session_mined;
            }
            set
            {
                current_session_mined = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentSessionMined"));
            }
        }

        public string CurrentCurrency { get; set; }
         
        public VideoCard()
        {
            current_session_mined = 0;
        }
       
      
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

        public MainWindow()
        {
            InitializeComponent();

            Cards = new ObservableCollection<VideoCard>() { new VideoCard() {
                Name = "MSI GTX 1060", Power = 600  } ,
                new VideoCard() { Name = "MSI GTX 1080 Ti", Power = 850  },
                new VideoCard() { Name = "Gigabyte GTX 1080 Ti", Power = 950 },
                new VideoCard() { Name = "INNO3D GTX 1080 Ti", Power = 800 },
                new VideoCard() { Name = "Asus PCI-Ex GeForce GT", Power = 250  }
            };

            MyList.ItemsSource = Cards;

        }

        public void BTCplus(object a)
        {
            VideoCard vc = a as VideoCard;
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
                vc.CurrentSessionMined++;
                
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
                vc.CurrentSessionMined++;
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
                vc.CurrentSessionMined++;
                Thread.Sleep(1000 - vc.Power);
            }
        }
       

        private void Start_Click(object sender, RoutedEventArgs e)
        {

            string ChoosenCurrency = (CardCurrency.SelectedItem as ComboBoxItem).Content.ToString();
            VideoCard card = MyList.SelectedItem as VideoCard;
            if (card.CurrentCurrency == ChoosenCurrency)
            {
                card.thr.Resume();
            }
            else
            {
                switch (ChoosenCurrency)
                {
                    case "BTC": { card.thr = new Thread(BTCplus); card.CurrentCurrency = "BTC";  break; }
                    case "KRB": { card.thr = new Thread(KRBplus); card.CurrentCurrency = "KRB"; break; }
                    case "ETH": { card.thr = new Thread(ETHplus); card.CurrentCurrency = "ETH"; break; }
                    default: { return; }
                }
                
                card.CurrentSessionMined = 0;
                card.thr.Start(card);
            }


            card.State = "Running";
            Start.IsEnabled = false;
            Stop.IsEnabled = true;
            MyList.Items.Refresh();

            MineInfo.Visibility = Visibility.Visible;
            MinedRun.SetBinding(Run.TextProperty, new Binding { Source = MyList.SelectedItem, Path = new PropertyPath("CurrentSessionMined"), Mode = BindingMode.TwoWay });
            CurrencyRun.SetBinding(Run.TextProperty, new Binding { Source = MyList.SelectedItem, Path = new PropertyPath("CurrentCurrency"), Mode = BindingMode.TwoWay });

        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {

            VideoCard card = MyList.SelectedValue as VideoCard;
            card.thr.Suspend();
            card.State = "Suspended";

            Start.IsEnabled = true;
            Stop.IsEnabled = false;
            MyList.Items.Refresh();
        }



        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (VideoCard v in Cards)
            {
                if (v.thr != null)
                {
                    if(v.State!="Suspended")
                    v.thr.Abort();
                }
            }
            Application.Current.Shutdown();
        }

        private void MyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MinedRun.SetBinding(Run.TextProperty, new Binding { Source = MyList.SelectedItem, Path = new PropertyPath("CurrentSessionMined"), Mode = BindingMode.TwoWay });
            CurrencyRun.SetBinding(Run.TextProperty, new Binding { Source = MyList.SelectedItem, Path = new PropertyPath("CurrentCurrency"), Mode = BindingMode.TwoWay });

            VideoCard vc = MyList.SelectedValue as VideoCard;
            if (vc.thr == null)
            {
                Start.IsEnabled = true;
                Stop.IsEnabled = false;
                return;
            }
            
            if (vc.State == "Running")
            {
                Start.IsEnabled = false;
                Stop.IsEnabled = true;
            }
            if (vc.State == "Suspended")
            {
                Start.IsEnabled = true;
                Stop.IsEnabled = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window_Closed(sender,e);
        }
    }
}

