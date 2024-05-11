using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;

namespace Airport
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new FlightViewModel();
        }

        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked != null)
            {
                string header = headerClicked.Column.Header as string;
                // DataContext to FlightViewModel type
                if (DataContext is FlightViewModel viewModel)
                {
                    viewModel.Sort(header);
                }
            }
        }
    }

    public class Flight : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int? _flightNumber;
        public int? FlightNumber
        {
            get { return _flightNumber; }
            set
            {
                _flightNumber = value;
                OnPropertyChanged(nameof(FlightNumber));
            }
        }

        private Brush _backgroundColor;
        public Brush BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                OnPropertyChanged(nameof(BackgroundColor));
            }
        }
        public void SetBackgroundColor()
        {
            if (Destination == null || Departure == null || ArrivalTime == DateTime.MinValue || DepartureTime == DateTime.MinValue)
            {
                BackgroundColor = Brushes.White;
            }
            else if ((Destination == Departure && Math.Abs((ArrivalTime - DepartureTime).TotalMinutes) < 5) ||
                     (Departure == Destination && Math.Abs((DepartureTime - ArrivalTime).TotalMinutes) < 5))
            {
                // If true - Yellow
                BackgroundColor = Brushes.DarkGoldenrod;
            }
            else
            {
                // In other cases - White (transparent)
                BackgroundColor = Brushes.Transparent;
            }
        }

        private string _airplaneName;
        public string AirplaneName
        {
            get { return _airplaneName; }
            set
            {
                _airplaneName = value;
                OnPropertyChanged(nameof(AirplaneName));
            }
        }

        private string _departure;
        public string Departure
        {
            get { return _departure; }
            set
            {
                _departure = value;
                OnPropertyChanged(nameof(Departure));
            }
        }

        private string _destination;
        public string Destination
        {
            get { return _destination; }
            set
            {
                _destination = value;
                OnPropertyChanged(nameof(Destination));
            }
        }

        private DateTime _departureTime;
        public DateTime DepartureTime
        {
            get { return _departureTime; }
            set
            {
                _departureTime = value;
                OnPropertyChanged(nameof(DepartureTime));
            }
        }

        private DateTime _arrivalTime;
        public DateTime ArrivalTime
        {
            get { return _arrivalTime; }
            set
            {
                _arrivalTime = value;
                OnPropertyChanged(nameof(ArrivalTime));
            }
        }

        private double? _ticketPrice;
        public double? TicketPrice
        {
            get { return _ticketPrice; }
            set
            {
                _ticketPrice = value;
                OnPropertyChanged(nameof(TicketPrice));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class FlightViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Flight> Flights { get; set; }

        public ICommand AddFlightCommand { get; }
        public ICommand DeleteFlightCommand { get; }

        private int? _newFlightNumber;
        public int? NewFlightNumber
        {
            get { return _newFlightNumber; }
            set
            {
                _newFlightNumber = value;
                OnPropertyChanged(nameof(NewFlightNumber));
            }
        }

        private string _newAirplaneName;
        public string NewAirplaneName
        {
            get { return _newAirplaneName; }
            set
            {
                _newAirplaneName = value;
                OnPropertyChanged(nameof(NewAirplaneName));
            }
        }

        private string _newDeparture;
        public string NewDeparture
        {
            get { return _newDeparture; }
            set
            {
                _newDeparture = value;
                OnPropertyChanged(nameof(NewDeparture));
            }
        }

        private string _newDestination;
        public string NewDestination
        {
            get { return _newDestination; }
            set
            {
                _newDestination = value;
                OnPropertyChanged(nameof(NewDestination));
            }
        }

        private DateTime? _newDepartureTime;
        public DateTime? NewDepartureTime
        {
            get { return _newDepartureTime; }
            set
            {
                _newDepartureTime = value;
                OnPropertyChanged(nameof(NewDepartureTime));
                _isDepartureTimeSet = value != null;
            }
        }

        private DateTime? _newArrivalTime;
        public DateTime? NewArrivalTime
        {
            get { return _newArrivalTime; }
            set
            {
                _newArrivalTime = value;
                OnPropertyChanged(nameof(NewArrivalTime));
                _isArrivalTimeSet = value != null;
            }
        }

        private double? _newTicketPrice;
        public double? NewTicketPrice
        {
            get { return _newTicketPrice; }
            set
            {
                _newTicketPrice = value;
                OnPropertyChanged(nameof(NewTicketPrice));
            }
        }

        private bool _isDepartureTimeSet;
        private bool _isArrivalTimeSet;

        private bool _isInvalidDateTime;
        public bool IsInvalidDateTime
        {
            get { return _isInvalidDateTime; }
            set
            {
                _isInvalidDateTime = value;
                OnPropertyChanged(nameof(IsInvalidDateTime));
            }
        }
        private void DeleteFlight(object parameter)
        {
            if (SelectedFlight != null)
            {
                Flights.Remove(SelectedFlight);

                foreach (var flight in Flights)
                {
                    bool similarFlightsFound = Flights.Count(f =>
                        (f.Destination == flight.Destination && Math.Abs((f.ArrivalTime - flight.ArrivalTime).TotalMinutes) < 5) ||
                        (f.Departure == flight.Departure && Math.Abs((f.DepartureTime - flight.DepartureTime).TotalMinutes) < 5)) > 1;

                    if (similarFlightsFound)
                    {
                        flight.BackgroundColor = new SolidColorBrush(Color.FromArgb(128, 255, 255, 0));
                    }
                    else
                    {
                        flight.BackgroundColor = Brushes.White;
                    }
                }
            }
        }

        private Flight _selectedFlight;

        public Flight SelectedFlight
        {
            get { return _selectedFlight; }
            set
            {
                if (_selectedFlight != value)
                {
                    _selectedFlight = value;
                    OnPropertyChanged(nameof(SelectedFlight));
                }
            }
        }

        public FlightViewModel()
        {
            Flights = new ObservableCollection<Flight>();
            AddFlightCommand = new RelayCommand(AddFlight);
            DeleteFlightCommand = new RelayCommand(DeleteFlight);
            NewFlightNumber = null;
            NewDestination = string.Empty;
            ResetNewDepartureTime();
            ResetNewArrivalTime();
            NewTicketPrice = null;
        }

        private void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            if (headerClicked != null)
            {
                string header = headerClicked.Column.Header as string;
                Sort(header);
            }
        }

        public void Sort(string sortBy)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(Flights);
            if (dataView != null)
            {
                if (dataView.SortDescriptions.Count > 0 && dataView.SortDescriptions[0].PropertyName == sortBy)
                {
                    SortDescription currentSort = dataView.SortDescriptions[0];
                    ListSortDirection newDirection = (currentSort.Direction == ListSortDirection.Ascending) ?
                        ListSortDirection.Descending : ListSortDirection.Ascending;

                    dataView.SortDescriptions.Clear();
                    dataView.SortDescriptions.Add(new SortDescription(sortBy, newDirection));
                }
                else
                {
                    dataView.SortDescriptions.Clear();
                    dataView.SortDescriptions.Add(new SortDescription(sortBy, ListSortDirection.Ascending));
                }
            }
        }


        //private Flight lastAddedFlight;
        private void AddFlight(object parameter)
        {

            // AirplaneName empty
            if (string.IsNullOrWhiteSpace(NewAirplaneName))
            {
                MessageBox.Show("Please enter the airplane name.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Departure empty
            if (string.IsNullOrWhiteSpace(NewDeparture))
            {
                MessageBox.Show("Please enter the departure location.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Destination empty
            if (string.IsNullOrWhiteSpace(NewDestination))
            {
                MessageBox.Show("Please enter the destination location.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ValidateDateTime(NewDepartureTime) || !ValidateDateTime(NewArrivalTime))
            {
                MessageBox.Show("Invalid date format! Please enter the date in the format dd.mm.yyyy hh:mm.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewDepartureTime > NewArrivalTime)
            {
                MessageBox.Show("Departure Time cannot be greater than Arrival Time.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newFlightNumber = NewFlightNumber;

            // Same flight number
            if (Flights.Any(flight => flight.FlightNumber == newFlightNumber))
            {
                MessageBox.Show("Flight number already exists! Please enter a different flight number.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newFlight = new Flight
            {
                FlightNumber = NewFlightNumber,
                AirplaneName = NewAirplaneName,
                Departure = NewDeparture,
                Destination = NewDestination,
                DepartureTime = NewDepartureTime.Value,
                ArrivalTime = NewArrivalTime.Value,
                TicketPrice = NewTicketPrice
            };

            Flights.Add(newFlight);

            bool similarFlightsFound = false;

            foreach (var flight in Flights)
            {
                if (flight == newFlight) continue; // Skip recently added flight

                if ((flight.Destination == newFlight.Destination && Math.Abs((flight.ArrivalTime - newFlight.ArrivalTime).TotalMinutes) < 5) ||
                    (flight.Departure == newFlight.Departure && Math.Abs((flight.DepartureTime - newFlight.DepartureTime).TotalMinutes) < 5))
                {
                    flight.BackgroundColor = new SolidColorBrush(Color.FromArgb(128, 255, 255, 0));
                    similarFlightsFound = true;
                }
            }

            if (similarFlightsFound)
            {
                MessageBox.Show("WARNING!!!\nFlights collisions found! Their rows are highlighted in yellow.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                newFlight.BackgroundColor = new SolidColorBrush(Color.FromArgb(128, 255, 255, 0));
            }

            ClearInputFields();
        }

        private bool ValidateDateTime(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return false;
            }

            var dateString = dateTime.Value.ToString("dd.MM.yyyy HH:mm");
            return DateTime.TryParseExact(dateString, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
        }

        private void ClearInputFields()
        {
            NewFlightNumber = null;
            NewAirplaneName = null;
            NewDeparture = null;
            NewDestination = null;
            ResetNewDepartureTime();
            ResetNewArrivalTime();
            NewTicketPrice = null;
        }

        private void ResetNewDepartureTime()
        {
            NewDepartureTime = null;
            _isDepartureTimeSet = false;
        }

        private void ResetNewArrivalTime()
        {
            NewArrivalTime = null;
            _isArrivalTimeSet = false;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }

}