using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

class ClientDemo
{
    private TcpClient _client;

    private StreamReader _sReader;
    private StreamWriter _sWriter;
    private Boolean _isConnected;

    public ClientDemo(String ipAddress, int portNum)
    {
        _client = new TcpClient();
        _client.Connect(ipAddress, portNum);

        HandleCommunication();
    }

    public void HandleCommunication()
    {
        _sReader = new StreamReader(_client.GetStream(), Encoding.ASCII);
        _sWriter = new StreamWriter(_client.GetStream(), Encoding.ASCII);

        _isConnected = true;
        String sData = null;
        String gelenData = null;
        String gonderilecekMesaj = null;
        while (_isConnected)
        {
            Console.Write("&gt; ");
            sData = Console.ReadLine();

            string clientAd = "Client1";
            int referans = 1;
            int tip = 2;
            int clientId = 1;

            gonderilecekMesaj = clientId + " " + clientAd + " " + sData + " " + referans + " " + tip;

            // write data and make sure to flush, or the buffer will continue to 
            // grow, and your data might not be sent when you want it, and will
            // only be sent once the buffer is filled.
            _sWriter.WriteLine(gonderilecekMesaj);
            _sWriter.Flush();

            gelenData = _sReader.ReadLine();
            Console.WriteLine("Client &gt; " + gelenData);

            // if you want to receive anything
            // String sDataIncomming = _sReader.ReadLine();
        }
    }

    static void Main(string[] args)
    {
        Console.WriteLine("Multi-Threaded TCP Server Demo");
        Console.WriteLine("Provide IP:");
        String ip = Console.ReadLine();

        Console.WriteLine("Provide Port:");
        int port = Int32.Parse(Console.ReadLine());

        ClientDemo client = new ClientDemo(ip, port);
    }
}