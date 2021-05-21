using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ScrollingListBox
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new ViewModel();

            InitializeComponent();
        }
    }

    public class ViewModel
    {
        private bool _running = true;
        public bool Running
        {
            get => _running;
            set
            {
                _running = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Running)));
            }
        }

        public ObservableCollection<DataItem> List { get; } = new ObservableCollection<DataItem>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModel()
        {
            Dispatcher mainThread = Dispatcher.CurrentDispatcher;
            Task.Run(() => UpdateData(mainThread));
        }

        private async Task UpdateData(Dispatcher mainThread)
        {
            await Task.Yield();
            while (true)
            {
                await Task.Delay(100);
                if (Running)
                {
                    await mainThread.InvokeAsync(() => List.Insert(0, new DataItem()));
                }
            }
        }
    }

    public class DataItem
    {
        public string Name { get; } = DateTime.Now.ToString("O");

        public override string ToString() => Name;
    }

    public class BetterListView : ListView
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new BetterListViewAutomationPeer(this);
        }
    }

    public class BetterListViewAutomationPeer : ListViewAutomationPeer
    {
        public BetterListViewAutomationPeer(BetterListView owner) : base(owner)
        {
        }

        protected override ItemAutomationPeer CreateItemAutomationPeer(object item)
        {
            return new BetterItemAutomationPeer(item, this);
        }
    }

    public class BetterItemAutomationPeer : ListBoxItemAutomationPeer
    {
        public BetterItemAutomationPeer(object item, BetterListViewAutomationPeer parentAutomationPeer) : base(item, parentAutomationPeer)
        { }

        protected override Point GetClickablePointCore()
        {
            if (this.IsOffscreen())
            {
                return new Point(-1, -1);
            }
            return base.GetClickablePointCore();
        }
    }
}
