using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace HIK.ASMES.IstemciTarafi
{
    /// <summary>
    /// Asenkron Soketli Mesajlaþma Ýstemcisi
    /// </summary>
    public class ASMESIstemcisi
    {
        // SABÝTLER ///////////////////////////////////////////////////////////

        private const byte BASLANGIC_BYTE = (byte)60; // <
        private const byte BITIS_BYTE = (byte)62; // >

        // PUBLIC ÖZELLÝKLER //////////////////////////////////////////////////

        /// <summary>
        /// ASMES Sunucusunun IP adresi
        /// </summary>
        public string ServerIpAdresi
        {
            get { return serverIpAdresi; }
        }
        private string serverIpAdresi;
        /// <summary>
        /// ASMES sunucusunun port numarasý
        /// </summary>
        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }
        private int serverPort;

        // OLAYLAR ////////////////////////////////////////////////////////////

        /// <summary>
        /// Sunucu ile olan baðlantý kapandýðýnda tetiklenir
        /// </summary>
        public event dgBaglantiKapatildi BaglantiKapatildi;
        /// <summary>
        /// Sunucudan yeni bir mesaj alýndýðýnda tetiklenir
        /// </summary>
        public event dgYeniMesajAlindi YeniMesajAlindi;

        // PRIVATE ALANLAR ////////////////////////////////////////////////////

        /// <summary>
        /// Sunucuya baðlantýyý saðlayan soket nesnesi
        /// </summary>
        private Socket istemciBaglantisi;
        /// <summary>
        /// Sunucuyla iletiþimi saðlayan temel akýþ nesnesi
        /// </summary>
        private NetworkStream agAkisi;
        /// <summary>
        /// Veri transfer etmek için kullanýlan akýþ nesnesi
        /// </summary>
        private BinaryWriter binaryYazici;
        /// <summary>
        /// Veri transfer etmek için kullanýlan akýþ nesnesi
        /// </summary>
        private BinaryReader binaryOkuyucu;
        /// <summary>
        /// Çalýþan thread'e referans
        /// </summary>
        private Thread thread;
        /// <summary>
        /// Ýstemci ile iletiþim devam ediyorsa true, aksi halde false
        /// </summary>
        private volatile bool calisiyor = false;

        // PUBLIC FONKSYONLAR /////////////////////////////////////////////////

        /// <summary>
        /// Bir ASMES Ýstemcisi oluþturur.
        /// </summary>
        /// <param name="serverIpAdresi">ASMES Sunucusunun IP adresi</param>
        /// <param name="serverPort">ASMES sunucusunun port numarasý</param>
        public ASMESIstemcisi(string serverIpAdresi, int serverPort)
        {
            this.serverIpAdresi = serverIpAdresi;
            this.serverPort = serverPort;
        }

        /// <summary>
        /// Sunucuya baðlantýyý kurar.
        /// </summary>
        /// <returns>baðlantý saðlandýysa true, aksi halde false</returns>
        public bool Baglan()
        {
            try
            {
                //sunucuya baðlan
                istemciBaglantisi = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(serverIpAdresi), serverPort);
                istemciBaglantisi.Connect(ipep);
                agAkisi = new NetworkStream(istemciBaglantisi);
                binaryOkuyucu = new BinaryReader(agAkisi, Encoding.ASCII);
                binaryYazici = new BinaryWriter(agAkisi, Encoding.ASCII);
                thread = new Thread(new ThreadStart(tCalis));
                calisiyor = true;
                thread.Start();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Sunucuya olan baðlantýyý kapatýr.
        /// </summary>
        public void BaglantiyiKes()
        {
            try
            {
                calisiyor = false;
                istemciBaglantisi.Close();
                thread.Join();
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Sunucuya bir mesaj yollamak içindir
        /// </summary>
        /// <param name="mesaj">Yollanacak mesaj</param>
        /// <returns>Ýþlemin baþarý durumu</returns>
        public bool MesajYolla(string mesaj)
        {
            try
            {
                //Mesajý byte dizisine çevirelim
                byte[] bMesaj = System.Text.Encoding.ASCII.GetBytes(mesaj);
                //Karþý tarafa gönderilecek byte dizisini oluþturalým
                byte[] b = new byte[bMesaj.Length + 2];
                Array.Copy(bMesaj, 0, b, 1, bMesaj.Length);
                b[0] = BASLANGIC_BYTE;
                b[b.Length - 1] = BITIS_BYTE;
                //Mesajý sokete yazalým
                binaryYazici.Write(b);
                agAkisi.Flush();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // PRIVATE FONKSYONLAR ////////////////////////////////////////////////

        /// <summary>
        /// Sunucudan mesajlarý dinleyen fonksyon
        /// </summary>
        private void tCalis()
        {
            //Her döngüde bir mesaj okunur
            while (calisiyor)
            {
                try
                {
                    //Baþlangýç Byte'ýný oku
                    byte b = binaryOkuyucu.ReadByte();
                    if (b != BASLANGIC_BYTE)
                    {
                        //Hatalý paket, baðlantýyý kes!
                        break;
                    }
                    //Mesajý oku
                    List<byte> bList = new List<byte>();
                    while ((b = binaryOkuyucu.ReadByte()) != BITIS_BYTE)
                    {
                        bList.Add(b);
                    }
                    string mesaj = System.Text.Encoding.ASCII.GetString(bList.ToArray());
                    //Yeni mesaj baþarýyla alýndý
                    yeniMesajAlindiTetikle(mesaj);
                }
                catch (Exception)
                {
                    //Hata oluþtu, baðlantýyý kes!
                    break;
                }
            }
            //Döngüden çýkýldýðýna göre baðlantý kapatýlmýþ demektir
            calisiyor = false;
            baglantiKapatildiTetikle();
        }

        private void baglantiKapatildiTetikle()
        {
            if (BaglantiKapatildi != null)
            {
                BaglantiKapatildi();
            }
        }

        private void yeniMesajAlindiTetikle(string mesaj)
        {
            if (YeniMesajAlindi != null)
            {
                YeniMesajAlindi(new MesajAlmaArgumanlari(mesaj));
            }
        }
    }
}