using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PropertyTools.Wpf;

namespace ALEWeek1
{
    public class ViewModel
    {
        public MainWindow MainWindow { get; set; }

        public ViewModel()
        {
            GenerateGraphCommand= new DelegateCommand(DoGenerateGraphCommand);
        }

        public ICommand GenerateGraphCommand { get; private set; }

        public void DoGenerateGraphCommand()
        {
            MainWindow.Parse(FormulaTextBox);
        }

        private Image _graph;
        public Image Graph
        {
            get { return _graph; }
            set
            {
                _graph = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> TruthTable { get; set; }

        public ObservableCollection<string> SimplifiedTurthTable { get; set; }

        private string _formulaTextBox;
        public string FormulaTextBox
        {
            get { return _formulaTextBox; }
            set { _formulaTextBox = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [Annotations.NotifyPropertyChangedInvocatorAttribute]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
