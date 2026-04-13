using DataModel;
using LinqToDB;
using Network;
using System.Net;
using System.Net.Sockets;

class Program : Receiver
{
    TcpListener listener = new TcpListener(IPAddress.Any, 12345);
    private WorldCitiesDB _db;
    private List<Worldcity> _worldcities;

    static void Main(string[] args)
    { 
        Program p = new Program();

        Console.ReadLine();
    }

    public Program()
    {
        DataOptions<WorldCitiesDB> options = new DataOptions<WorldCitiesDB>(new DataOptions().UseSQLite("Data Source=worldcities.sqlite"));
        _db = new WorldCitiesDB(options);

        _worldcities = _db.Worldcities.ToList();

        listener.Start();

        ThreadPool.QueueUserWorkItem(o =>
        {
            while (true)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Transfer<MSG> t = new Transfer<MSG>(client);
                    t.OnMessageReceived += (sender, msg) => ReceiveMessage(msg, t);
                    t.OnDisconnected += (sender, e) => TransferDisconnected(t);
                }
                catch { }
            }
        });

    }

    public void ReceiveMessage(MSG m, Transfer<MSG> t)
    {
        if (m.type == MSG.Type.SEARCH)
        {
            var result = _worldcities.Where(c => c.City.Contains(m.message));
            string message = string.Join(";", result.Select(c => $"{c.City}|{c.Country}|{c.Lng}|{c.Lat}"));
            t.Send(new MSG { type = MSG.Type.RESULT, message = message });

            Console.WriteLine("Sent: \n" + message);
        }
    }

    public void TransferDisconnected(Transfer<MSG> t)
    {

    }

    public void AddDebugInfo(Transfer<MSG> t, String m, bool sent)
    {

    }
}
