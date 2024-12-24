using Flurl.Http;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace MauiApp2
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainViewModel();
        }
    }
    class MainViewModel : INotifyPropertyChanged
    {
        private Dictionary<DateTime, Root> _history = new Dictionary<DateTime, Root>();
        public MainViewModel()
        {
            bool datesearh = true;
            SelectedDate = DateTime.Now;
            ValutesList = new ObservableCollection<ValuteItem>();
            Refresh = new Command(async () =>
            {
                GetValutes();
            });

            if (Refresh == null)
            {
                throw new InvalidOperationException("Command Refresh is not initialized.");
            }

            Refresh.Execute(null);
        }


        private Root _apiResult;
        private ErrorAnswer _apiError;
        public ObservableCollection<ValuteItem> ValutesList { get; }

        private ValuteItem _selectedFirstValute;
        public ValuteItem SelectedFirstValute
        {
            get => _selectedFirstValute;
            set
            {
                if (_selectedFirstValute == value) return;
                _selectedFirstValute = value;
                OnPropertyChanged();
                Calculate(true);
            }
        }

        private ValuteItem _selectedSecondValute;
        public ValuteItem SelectedSecondValute
        {
            get => _selectedSecondValute;
            set
            {
                if (_selectedSecondValute == value) return;
                _selectedSecondValute = value;
                OnPropertyChanged();
                Calculate(false);
            }
        }

        private double _valueInput;
        public double ValueInput
        {
            get => _valueInput;
            set
            {
                if (_valueInput == value) return;
                _valueInput = value;
                OnPropertyChanged();
                Calculate(true);
            }
        }

        private double _valueOutput;
        public double ValueOutput
        {
            get => _valueOutput;
            set
            {
                if (_valueOutput == value) return;
                _valueOutput = value;
                OnPropertyChanged();
                Calculate(false);
            }
        }

        private DateTime _selectedDate;
        private ValuteItem _previousFirstValute;
        private ValuteItem _previousSecondValute;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                if (_selectedDate == value) return;
                _selectedDate = value;
                OnPropertyChanged();
                ValueOutput = 0;
                GetValutes();
            }
        }

        public async void GetValutes()
        {
            DateTime currentDate = SelectedDate;
            bool datesearch = true;
            _previousFirstValute = SelectedFirstValute;
            _previousSecondValute = SelectedSecondValute;
            while (datesearch)
            {
                try
                {
                    string urls = $"https://www.cbr-xml-daily.ru/archive/{currentDate:yyyy}/{currentDate:MM}/{currentDate:dd}/daily_json.js";
                    _apiResult = await urls.GetJsonAsync<Root>();
                    _history[currentDate] = _apiResult;
                    ValutesList.Clear();

                    var rub = new ValuteItem
                    {
                        ID = "R00000",
                        NumCode = "643",
                        CharCode = "RUB",
                        Nominal = 1,
                        Name = "Российский рубль",
                        Value = 1,
                        Previous = 1
                    };
                    ValutesList.Add(rub);

                    foreach (var item in _apiResult.Valute.AllItems)
                    {
                        ValutesList.Add(item);
                    }


                    if (_previousFirstValute != null && _previousSecondValute != null)
                    {
                        try
                        {
                            SelectedFirstValute = ValutesList.FirstOrDefault<ValuteItem>(x => x.CharCode == _previousFirstValute.CharCode);
                            SelectedSecondValute = ValutesList.FirstOrDefault<ValuteItem>(x => x.CharCode == _previousSecondValute.CharCode);
                        }
                        catch (System.NullReferenceException ex)
                        {
                            
                        }
                        
                    }

                    datesearch = false;
                }
                catch (FlurlHttpException ex)
                {
                    currentDate = currentDate.AddDays(-1);
                    SelectedDate = currentDate;
                }
            }
        }
        private bool _isCalculating;

        private void Calculate(bool isFirstValute)
        {
            if (_isCalculating) return; 

            try
            {
                _isCalculating = true; 

                if (SelectedSecondValute != null && isFirstValute && SelectedFirstValute != null)
                {
                    ValueOutput = Math.Round(ValueInput * (SelectedFirstValute.Value / SelectedFirstValute.Nominal) / (SelectedSecondValute.Value / SelectedSecondValute.Nominal), 2);
                }
                else if (SelectedFirstValute != null && !isFirstValute && SelectedSecondValute != null)
                {
                    ValueInput = Math.Round(ValueOutput * (SelectedSecondValute.Value / SelectedSecondValute.Nominal) / (SelectedFirstValute.Value / SelectedFirstValute.Nominal), 2);
                }
            }
            finally
            {
                _isCalculating = false; 
            }
        }


        public Command Refresh { get; }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Root
    {
        public DateTime Date { get; set; }
        public DateTime PreviousDate { get; set; }
        public string PreviousURL { get; set; }
        public DateTime Timestamp { get; set; }
        public Valute Valute { get; set; }
    }

    public class Valute
    {
        public ValuteItem? AUD { get; set; }
        public ValuteItem? AZN { get; set; }
        public ValuteItem? GBP { get; set; }    
        public ValuteItem? AMD { get; set; }
        public ValuteItem? BYN { get; set; }
        public ValuteItem? BGN { get; set; }
        public ValuteItem? BRL { get; set; }
        public ValuteItem? HUF { get; set; }
        public ValuteItem? VND { get; set; }
        public ValuteItem? HKD { get; set; }
        public ValuteItem? GEL { get; set; }
        public ValuteItem? DKK { get; set; }
        public ValuteItem? AED { get; set; }
        public ValuteItem? USD { get; set; }
        public ValuteItem? EUR { get; set; }
        public ValuteItem? EGP { get; set; }
        public ValuteItem? INR { get; set; }
        public ValuteItem? IDR { get; set; }
        public ValuteItem? KZT { get; set; }
        public ValuteItem? CAD { get; set; }
        public ValuteItem? QAR { get; set; }
        public ValuteItem? KGS { get; set; }
        public ValuteItem? CNY { get; set; }
        public ValuteItem? MDL { get; set; }
        public ValuteItem? NZD { get; set; }
        public ValuteItem? NOK { get; set; }
        public ValuteItem? PLN { get; set; }
        public ValuteItem? RON { get; set; }
        public ValuteItem? XDR { get; set; }
        public ValuteItem? SGD { get; set; }
        public ValuteItem? TJS { get; set; }
        public ValuteItem? THB { get; set; }
        public ValuteItem? TRY { get; set; }
        public ValuteItem? TMT { get; set; }
        public ValuteItem? UZS { get; set; }
        public ValuteItem? UAH { get; set; }
        public ValuteItem? CZK { get; set; }
        public ValuteItem? SEK { get; set; }
        public ValuteItem? CHF { get; set; }
        public ValuteItem? RSD { get; set; }
        public ValuteItem? ZAR { get; set; }
        public ValuteItem? KRW { get; set; }
        public ValuteItem? JPY { get; set; }
        

        public IEnumerable<ValuteItem> AllItems
        {
            get
            {
                var result = new List<ValuteItem>
                {
                    AUD,
                    AZN,
                    GBP,
                    AMD,
                    BYN,
                    BGN,
                    BRL,
                    HUF,
                    VND,
                    HKD,
                    GEL,
                    DKK,
                    AED,
                    USD,
                    EUR,
                    EGP,
                    INR,
                    IDR,
                    KZT,
                    CAD,
                    QAR,
                    KGS,
                    CNY,
                    MDL,
                    NZD,
                    NOK,
                    PLN,
                    RON,
                    XDR,
                    SGD,
                    TJS,
                    THB,
                    TRY,
                    TMT,
                    UZS,
                    UAH,
                    CZK,
                    SEK,
                    CHF,
                    RSD,
                    ZAR,
                    KRW,
                    JPY
                };
                


                return result.Where(x=> x != null);
            }
        }
    }


    public class ValuteItem
    {
        public string ID { get; set; }
        public string NumCode { get; set; }
        public string CharCode { get; set; }
        public int Nominal { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public double Previous { get; set; }
    }


    public class ErrorAnswer
    {
        public string error { get; set; }
        public int code { get; set; }
        public string explanation { get; set; }
    }
}
