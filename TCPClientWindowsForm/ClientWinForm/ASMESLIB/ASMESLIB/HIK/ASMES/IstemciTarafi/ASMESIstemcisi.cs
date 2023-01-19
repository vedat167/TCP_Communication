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
    /// Asenkron Soketli Mesajla�ma �stemcisi
    /// </summary>
    public class ASMESIstemcisi
    {
        // SAB�TLER ///////////////////////////////////////////////////////////

        private const byte BASLANGIC_BYTE = (byte)60; // <
        private const byte BITIS_BYTE = (byte)62; // >

        // PUBLIC �ZELL�KLER //////////////////////////////////////////////////

        /// <summary>
        /// ASMES Sunucusunun IP adresi
        /// </summary>
        public string ServerIpAdresi
        {
            get { return serverIpAdresi; }
        }
        private string serverIpAdresi;
        /// <summary>
        /// ASMES sunucusunun port numaras�
        /// </summary>
        public int ServerPort
        {
            get { return serverPort; }
            set { serverPort = value; }
        }
        private int serverPort;

        // OLAYLAR ////////////////////////////////////////////////////////////

        /// <summary>
        /// Sunucu ile olan ba�lant� kapand���nda tetiklenir
        /// </summary>
        public event dgBaglantiKapatildi BaglantiKapatildi;
        /// <summary>
        /// Sunucudan yeni bir mesaj al�nd���nda tetiklenir
        /// </summary>
        public event dgYeniMesajAlindi YeniMesajAlindi;

        // PRIVATE ALANLAR ////////////////////////////////////////////////////

        /// <summary>
        /// Sunucuya ba�lant�y� sa�layan soket nesnesi
        /// </summary>
        private Socket istemciBaglantisi;
        /// <summary>
        /// Sunucuyla ileti�imi sa�layan temel ak�� nesnesi
        /// </summary>
        private NetworkStream agAkisi;
        /// <summary>
        /// Veri transfer etmek i�in kullan�lan ak�� nesnesi
        /// </summary>
        private BinaryWriter binaryYazici;
        /// <summary>
        /// Veri transfer etmek i�in kullan�lan ak�� nesnesi
        /// </summary>
        private BinaryReader binaryOkuyucu;
        /// <summary>
        /// �al��an thread'e referans
        /// </summary>
        private Thread thread;
        /// <summary>
        /// �stemci ile ileti�im devam ediyorsa true, aksi halde false
        /// </summary>
        private volatile bool calisiyor = false;

        // PUBLIC FONKSYONLAR /////////////////////////////////////////////////

        /// <summary>
        /// Bir ASMES �stemcisi olu�turur.
        /// </summary>
        /// <param name="serverIpAdresi">ASMES Sunucusunun IP adresi</param>
        /// <param name="serverPort">ASMES sunucusunun port numaras�</param>
        public ASMESIstemcisi(string serverIpAdresi, int serverPort)
        {
            this.serverIpAdresi = serverIpAdresi;
            this.serverPort = serverPort;
        }

        /// <summary>
        /// Sunucuya ba�lant�y� kurar.
        /// </summary>
        /// <returns>ba�lant� sa�land�ysa true, aksi halde false</returns>
        public bool Baglan()
        {
            try
            {
                //sunucuya ba�lan
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
        /// Sunucuya olan ba�lant�y� kapat�r.
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
        /// Sunucuya bir mesaj yollamak i�indir
        /// </summary>
        /// <param name="mesaj">Yollanacak mesaj</param>
        /// <returns>��lemin ba�ar� durumu</returns>
        public bool MesajYolla(string mesaj)
        {
            try
            {
                //Mesaj� byte dizisine �evirelim
                byte[] bMesaj = System.Text.Encoding.ASCII.GetBytes(mesaj);
                //Kar�� tarafa g�nderilecek byte dizisini olu�tural�m
                byte[] b = new byte[bMesaj.Length + 2];
                Array.Copy(bMesaj, 0, b, 1, bMesaj.Length);
                b[0] = BASLANGIC_BYTE;
                b[b.Length - 1] = BITIS_BYTE;
                //Mesaj� sokete yazal�m
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
        /// Sunucudan mesajlar� dinleyen fonksyon
        /// </summary>
        private void tCalis()
        {
            //Her d�ng�de bir mesaj okunur
            while (calisiyor)
            {
                try
                {
                    //Ba�lang�� Byte'�n� oku
                    byte b = binaryOkuyucu.ReadByte();
                    if (b != BASLANGIC_BYTE)
                    {
                        //Hatal� paket, ba�lant�y� kes!
                        break;
                    }
                    //Mesaj� oku
                    List<byte> bList = new List<byte>();
                    while ((b = binaryOkuyucu.ReadByte()) != BITIS_BYTE)
                    {
                        bList.Add(b);
                    }
                    string mesaj = System.Text.Encoding.ASCII.GetString(bList.ToArray());
                    //Yeni mesaj ba�ar�yla al�nd�
                    yeniMesajAlindiTetikle(mesaj);
                }
                catch (Exception)
                {
                    //Hata olu�tu, ba�lant�y� kes!
                    break;
                }
            }
            //D�ng�den ��k�ld���na g�re ba�lant� kapat�lm�� demektir
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