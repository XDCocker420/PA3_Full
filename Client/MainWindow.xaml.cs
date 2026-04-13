using Client.Model;
using Network;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, Receiver
    {
        private Transfer<MSG> _transfer;
        private ViewModel vm = new ViewModel();

        public MainWindow()
        {
            InitializeComponent();

            DataContext = vm;

            TcpClient client = new TcpClient("127.0.0.1", 12345);
            _transfer = new Transfer<MSG>(client);

            _transfer.OnMessageReceived += (s, msg) => ReceiveMessage(msg, _transfer);
            _transfer.OnDisconnected += (s, e) => TransferDisconnected(_transfer);
            WorldMap.SizeChanged += (s, e) => RedrawCities();
        }

        public void ReceiveMessage(MSG m, Transfer<MSG> t)
        {
            if (m.type == MSG.Type.RESULT)
            {
                string[] cities = m.message.Split(";");

                Dispatcher.Invoke(() =>
                {
                    foreach (string city in cities)
                    {
                        string[] cityParts = city.Split("|");
                        vm.SearchCities.Add(new City { CityName = cityParts[0], Country = cityParts[1], Lng = double.Parse(cityParts[2]), Lat = double.Parse(cityParts[3]) });
                    }
                    RedrawCities();
                });
            }
        }

        public void TransferDisconnected(Transfer<MSG> t)
        {

        }

        public void AddDebugInfo(Transfer<MSG> t, String m, bool sent)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchBox.Text;

            if (vm.SearchCities != null)
            {
                vm.SearchCities.Clear();
            }

            
            WorldMap.Children.Clear();

            _transfer.Send(new MSG { type = MSG.Type.SEARCH, message = searchText });
        }

        private void DrawCity(double lng, double lat)
        {
            double x = (lng + 180) / 360 * WorldMap.ActualWidth;
            double y = (90 - lat) / 180 * WorldMap.ActualHeight;

            Ellipse dot = new Ellipse() { Width = 8, Height = 8, Fill = Brushes.Red };
            Canvas.SetLeft(dot, x);
            Canvas.SetTop(dot, y);
            WorldMap.Children.Add(dot);
        } 

        private void RedrawCities()
        {
            WorldMap.Children.Clear();

            foreach (City c in vm.SearchCities)
            {
                DrawCity(c.Lng, c.Lat);
            }
        }

        // Verschieben
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            City selectedCity = SearchResultBox.SelectedItem as City;
            if (selectedCity == null)
            {
                return;
            }

            if (vm.PathCities.Contains(selectedCity))
            {
                return;
            }

            vm.PathCities.Add(selectedCity);
        }

        // Berechnen
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            List<City> cities = vm.PathCities.ToList();
            if (cities.Count < 2) return;

            List<List<City>> allPermutations = new List<List<City>>();
            GetPermutations(new List<City>(), cities, allPermutations);

            List<City> bestRoute = null;
            double bestDistance = double.MaxValue;

            foreach (List<City> perm in allPermutations)
            {
                double d = TotalDistance(perm);
                if (d < bestDistance)
                {
                    bestDistance = d;
                    bestRoute = perm;
                }
            }

            DrawRoute(bestRoute);
            MessageBox.Show($"Kürzeste Route: {bestDistance:F0} km");
        }

        private void GetPermutations(List<City> current, List<City> remaining, List<List<City>> result)
        {
            if (remaining.Count == 0)
            {
                result.Add(new List<City>(current));
                return;
            }
            foreach (City c in remaining)
            {
                current.Add(c);
                GetPermutations(current, remaining.Where(x => x != c).ToList(), result);
                current.RemoveAt(current.Count - 1);
            }
        }

        private double TotalDistance(List<City> route)
        {
            double total = 0;
            for (int i = 0; i < route.Count - 1; i++)
                total += Distance(route[i], route[i + 1]);
            total += Distance(route[route.Count - 1], route[0]);
            return total;
        }

        private double Distance(City a, City b)
        {
            return Math.Sqrt(Math.Pow((a.Lat - b.Lat), 2) + Math.Pow((a.Lng - b.Lng), 2));
        }

        private void DrawRoute(List<City> route)
        {
            RedrawCities();
            for (int i = 0; i < route.Count; i++)
            {
                City from = route[i];
                City to = route[(i + 1) % route.Count];

                double x1 = (from.Lng + 180) / 360 * WorldMap.ActualWidth;
                double y1 = (90 - from.Lat) / 180 * WorldMap.ActualHeight;
                double x2 = (to.Lng + 180) / 360 * WorldMap.ActualWidth;
                double y2 = (90 - to.Lat) / 180 * WorldMap.ActualHeight;

                Line line = new Line
                {
                    X1 = x1, Y1 = y1,
                    X2 = x2, Y2 = y2,
                    Stroke = Brushes.Yellow,
                    StrokeThickness = 2
                };
                WorldMap.Children.Add(line);
            }
        }
    }
}
