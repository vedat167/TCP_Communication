using System;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace ClientWinForm
{

    public partial class Form1 : Form
    {
        TcpListener Listener = null;
        #region Degiskenler

        private TcpClient client;

        private StreamReader sReader;
        private StreamWriter sWriter;
        private Boolean isConnected;

        public string SendingFilePath = string.Empty;
        private const int BufferSize = 1024;
        public static Thread T = null;

        public static bool FileTransferOk = false;

        #endregion
        #region SunucuBaglantiDataTransfer
        private void btnGonder_Click(object sender, EventArgs e) // sunucu-client arası data transfer
        {
            string ip = txtip.Text; // gönderilecek ip
            int port = Int32.Parse(txtPort.Text); // gönderilecek port

            if ((txtip.Text != null) && (txtPort.Text != null) && (txtClient.Text != null) && (txtVeri != null) && (cmbislem.SelectedItem != null))
            {
                client = new TcpClient(); // client oluştur
                client.Connect(ip, port); // bağlanıtıyı sağla

                sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
                sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);

                isConnected = true;
                string gelenData = "";
                string gonderilecekData = "";

                gonderilecekData = txtClient.Text + " " + txtVeri.Text + " " + cmbislem.SelectedItem.ToString();
                sWriter.WriteLine(gonderilecekData); // gönderilecek datayı yazma
                sWriter.Flush();

                gelenData = sReader.ReadLine(); // gelen datayı okuma

                if (gelenData.Contains("client_list*"))
                {
                    String clist = gelenData.Split('*')[1];
                    string[] ip_list = clist.Split(',');

                    listBox1.Items.Add(ip_list[0]);
                }

                LblMesaj.Text = gelenData.ToString();
            }

            else
            {
                MessageBox.Show("Lütfen Boş Alan Bırakmayınız");
            }
        }

        #endregion

        #region SetListener
        void SetListener()
        {


            try
            {
                string ipAddress = "192.168.0.149";
                IPAddress address = IPAddress.Parse(ipAddress);
                Byte[] bytes = address.GetAddressBytes();
                for (int i = 0; i < bytes.Length; i++)
                {
                    Console.Write(bytes[i]);
                }
                Listener = new TcpListener(address, Convert.ToInt32("5555")); // Listener oluştur

                //Listener.Start(); // Dinlemeye başla
            }

            catch (Exception ex)
            {
                // MessageBox.Show(ex.ToString());
              
            }
        }
        #endregion
        #region ReceiveFile
        public void ReceiveTCP(int portN) // File Receive
        {
         
        
            byte[] dataKaydet = new byte[BufferSize]; // gelen data için dizi
            int RecBytes;
            string fileName = null;

            for (; ; )
            {
                TcpClient client = null; // client oluştur
                NetworkStream netstream = null;

                try
                {

                    if (Listener.Pending()) // Dinleme olduğu sürece
                    {
                        client = Listener.AcceptTcpClient(); // client dinlemeye başla                    
                        netstream = client.GetStream();

                        BinaryReader reader = new BinaryReader(client.GetStream()); // gelen dosyayı oku
                        string ss = reader.ReadString();

                        if (FileTransferOk == true) // Doya transferini başlat
                        {
                            String[] Keys = ss.Replace("#FileSend#", "").Split('/'); // Gelen etiketi kontrol et

                            //BinaryWriter writer = new BinaryWriter(clientReceive.GetStream());
                            //writer.Write("#FileSend#FileName:Vedat/SendClientIP:192.168.0.149/SendPort:5555");

                            NetworkStream ns = client.GetStream();
                            ns.Write(dataKaydet, 0, dataKaydet.Length);

                            int totalrecbytes = 0;
                            fileName = DateTime.Now.ToString("DDmmYYYYHHmmSSS"); // Kaydedilecek dosya adı
                            FileStream Fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + fileName, FileMode.OpenOrCreate, FileAccess.Write); // Dosyayı clientta oluştur

                            while ((RecBytes = netstream.Read(dataKaydet, 0, dataKaydet.Length)) > 0) // Dosyayı clienta yaz
                            {
                                Fs.Write(dataKaydet, 0, RecBytes);
                                totalrecbytes += RecBytes;
                            }

                            Fs.Close();

                            FileTransferOk = false;
                        }

                        if (ss.Contains("#FileSend#"))
                        {
                            FileTransferOk = true;
                        }
                        
                        //netstream.Close();
                        //client.Close();
                    }
                }

                catch (Exception ex)
                {
                   // MessageBox.Show(ex.ToString());
                    //netstream.Close();
                }
                Listener.Stop();
            }
        }

        #endregion
        #region StartReceivingFile
        public void StartReceiving()
        {
            ReceiveTCP(5555);
        }

        #endregion
        #region DosyaSec
        private void button1_Click(object sender, EventArgs e) // Dosya Seç
        {
            OpenFileDialog Dlg = new OpenFileDialog();
            Dlg.Filter = "All Files (*.*)|*.*";
            Dlg.CheckFileExists = true;
            Dlg.Title = "Choose a File";
            Dlg.InitialDirectory = @"C:\";

            if (Dlg.ShowDialog() == DialogResult.OK)
            {
                SendingFilePath = Dlg.FileName;
            }
        }

        #endregion
        #region SendFileTcpMetodu
        public void SendTCP(string dosyaYolu, string ip, Int32 port) // Send file metodu
        {
            byte[] SendingBuffer = null;
            TcpClient client = null; // client oluştur
            LblMesaj.Text = "";
            NetworkStream netStream = null;
         
            try
            {
                client = new TcpClient(ip, port);
                netStream = client.GetStream();

                FileStream Fs = new FileStream(dosyaYolu, FileMode.Open, FileAccess.Read);// Dosya oku ve yolu belirt

                int paketSayisi = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(BufferSize)));
                int toplamUzunluk = (int)Fs.Length, CurrentPacketLength;

                for (int i = 0; i < paketSayisi; i++)
                {
                    if (toplamUzunluk > BufferSize)
                    {
                        CurrentPacketLength = BufferSize;
                        toplamUzunluk = toplamUzunluk - CurrentPacketLength;
                    }
                    else
                    {
                        CurrentPacketLength = toplamUzunluk;
                    }
                        
                    SendingBuffer = new byte[CurrentPacketLength];
                    Fs.Read(SendingBuffer, 0, CurrentPacketLength);
                    netStream.Write(SendingBuffer, 0, (int)SendingBuffer.Length);

                }

                Fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                netStream.Close();
                client.Close();

            }
            Listener.Stop();
        }

        #endregion
        #region StartSendingFile
        String ipAddress = "192.168.0.149";
        private void btnFileGonder_Click(object sender, EventArgs e) // Send file butonu
        {
            string ip = txtip.Text;
            int port = Int32.Parse(txtPort.Text);

            client = new TcpClient(); // client oluştur
            client.Connect(ip, port); // bağlanıtıyı sağla

            if (SendingFilePath != string.Empty)
            {
                BinaryWriter writer = new BinaryWriter(client.GetStream());
                writer.Write("#FileSend#FileName:Vedat/SendClientIP:"+ipAddress+"/SendPort:5555");

                SendTCP(SendingFilePath, ip, port);
                LblMesajFile.Text = "(Dosya Gönderildi)";
            }

            else
            {
                MessageBox.Show("Select a file", "Warning");
            }

            //ThreadStart Ts = new ThreadStart(StartReceiving);
            //T = new Thread(Ts);
            //T.Start();

            
        }

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetListener();
            ThreadStart Ts = new ThreadStart(StartReceiving);
            T = new Thread(Ts);
            T.Start();

          
        }
    }
}

