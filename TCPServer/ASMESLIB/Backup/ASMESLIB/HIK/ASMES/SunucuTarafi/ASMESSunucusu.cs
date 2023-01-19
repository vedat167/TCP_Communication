using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.IO;

namespace HIK.ASMES.SunucuTarafi
{
    /// <summary>
    /// Asenkron Soketli Mesajlaþma Sunucusu
    /// </summary>
    public class ASMESSunucusu
    {
        // PUBLIC ÖZELLÝKLER //////////////////////////////////////////////////

        /// <summary>
        /// Dinlenen Port
        /// </summary>
        public int Port
        {
            get { return port; }
        }
        private int port;

        // OLAYLAR ////////////////////////////////////////////////////////////

        /// <summary>
        /// Sunucuya yeni bir istemci baðlantýðýnda tetiklenir.
        /// </summary>
        public event dgYeniIstemciBaglandi YeniIstemciBaglandi;
        /// <summary>
        /// Sunucuya baðlý bir istemci baðlantýsý kapatýldýðýnda tetiklenir.
        /// </summary>
        public event dgIstemciBaglantisiKapatildi IstemciBaglantisiKapatildi;
        /// <summary>
        /// Bir istemciden yeni bir mesaj alýndýðýnda tetiklenir.
        /// </summary>
        public event dgIstemcidenYeniMesajAlindi IstemcidenYeniMesajAlindi;

        // PRIVATE ALANLAR ////////////////////////////////////////////////////

        /// <summary>
        /// En son baðlantý saðlanan istemci ID'si
        /// </summary>
        private long sonIstemciID = 0;
        /// <summary>
        /// O anda baðlantý kurulmuþ olan istemcilerin listesi
        /// </summary>
        private SortedList<long, Istemci> istemciler;
        /// <summary>
        /// Sunucunun çalýþma durumu
        /// </summary>
        private volatile bool calisiyor;
        /// <summary>
        /// Senkronizasyonda kullanýlacak nesne
        /// </summary>
        private object objSenk = new object();
        /// <summary>
        /// Baðlantý dinleyen nesne
        /// </summary>
        private BaglantiDinleyici baglantiDinleyici;

        // PUBLIC FONKSYONLAR /////////////////////////////////////////////////
        
        /// <summary>
        /// Bir ASMES sunucusu oluþturur
        /// </summary>
        /// <param name="port">Dinlenecek port</param>
        public ASMESSunucusu(int port)
        {
            this.port = port;
            this.istemciler = new SortedList<long, Istemci>();
            this.baglantiDinleyici = new BaglantiDinleyici(this, port);
        }

        /// <summary>
        /// Sunucunun çalýþmasýný baþlatýr
        /// </summary>
        public void Baslat()
        {
            //Eðer zaten çalýþýyorsa iþlem yapmadan çýk
            if (calisiyor)
            {
                return;
            }
            //Dinleyiciyi baþlat
            if (!baglantiDinleyici.Baslat())
            {
                return;
            }
            //Çalýþýyor bayraðýný iþaretle
            calisiyor = true;
        }

        /// <summary>
        /// Sunucunun çalýþmasýný durdurur
        /// </summary>
        public void Durdur()
        {
            //Dinleyiciyi durdur
            baglantiDinleyici.Durdur();
            //Tüm istemcileri durdur
            calisiyor = false;
            try
            {
                IList<Istemci> istemciListesi = istemciler.Values;
                foreach (Istemci ist in istemciListesi)
                {
                    ist.Durdur();
                }
            }
            catch (Exception)
            {

            }
            //Ýstemcileri temizle
            istemciler.Clear();
            //Çalýþýyor bayraðýndaki iþareti kaldýr
            calisiyor = false;
        }

        /// <summary>
        /// Bir istemciye bir mesaj yollar
        /// </summary>
        /// <param name="mesaj">Yollanacak mesaj</param>
        /// <returns>Ýþlemin baþarý durumu</returns>
        public bool MesajYolla(IIstemci istemci, string mesaj)
        {
            return istemci.MesajYolla(mesaj);
        }

        // PRIVATE FONKSYONLAR ////////////////////////////////////////////////

        /// <summary>
        /// Yeni bir istemci baðlantýðýnda buraya gönderilir.
        /// </summary>
        /// <param name="istemciSoketi">Yeni baðlanan istemci soketi</param>
        private void yeniIstemciSoketiBaglandi(Socket istemciSoketi)
        {
            //Yeni baðlanan istemciyi listeye ekle
            Istemci istemci = null;
            lock (objSenk)
            {
                istemci = new Istemci(this, istemciSoketi, ++sonIstemciID);
                istemciler.Add(istemci.IstemciID, istemci);
            }
            //Ýstemciyi çalýþmaya baþlat
            istemci.Baslat();
            //YeniIstemciBaglandi olayýný tetikle
            if (YeniIstemciBaglandi != null)
            {
                YeniIstemciBaglandi(new IstemciBaglantiArgumanlari(istemci));
            }
        }

        /// <summary>
        /// Bir Istemci nesnesi bir mesaj aldýðýnda buraya iletir
        /// </summary>
        /// <param name="istemci">Paketi alan Istemci nesnesi</param>
        /// <param name="mesaj">Mesaj nesnesi</param>
        private void yeniIstemciMesajiAlindi(Istemci istemci, string mesaj)
        {
            if (IstemcidenYeniMesajAlindi != null)
            {
                IstemcidenYeniMesajAlindi(new IstemcidenMesajAlmaArgumanlari(istemci, mesaj));
            }
        }

        /// <summary>
        /// Bir Istemci nesnesiyle iliþkili baðlantý kapatýldýðýnda, burasý çaðýrýlýr
        /// </summary>
        /// <param name="istemci">Kapatýlan istemci baðlantýsý</param>
        private void istemciBaglantisiKapatildi(Istemci istemci)
        {
            //IstemciBaglantisiKapatildi olayýný tetikle
            if (IstemciBaglantisiKapatildi != null)
            {
                IstemciBaglantisiKapatildi(new IstemciBaglantiArgumanlari(istemci));
            }
            //Kapanan istemciyi listeden çýkar
            if (calisiyor)
            {
                lock (objSenk)
                {
                    if (istemciler.ContainsKey(istemci.IstemciID))
                    {
                        istemciler.Remove(istemci.IstemciID);
                    }
                }
            }
        }

        // ALT SINIFLAR ///////////////////////////////////////////////////////
        
        /// <summary>
        /// Ayrý bir thread olarak çalýþýp gelen soket baðlantýlarýný kabul ederek 
        /// ASMESSunucusu modülüne ileten sýnýf.
        /// </summary>
        private class BaglantiDinleyici
        {
            /** Gelen baðlantýlarýn aktarýlacaðý modül */
            private ASMESSunucusu sunucu;
            /** Gelen baðlantýlarý dinlemek için sunucu soketi */
            private TcpListener dinleyiciSoket;
            /** Dinlenen portun numarasý */
            private int port;
            /** thread'in çalýþmasýný kontrol eden bayrak */
            private volatile bool calisiyor = false;
            /** çalýþan thread'e referans */
            private volatile Thread thread;

            /// <summary>
            /// Kurucu fonksyon.
            /// </summary>
            /// <param name="port">Dinlenecek port no</param>
            public BaglantiDinleyici(ASMESSunucusu sunucu, int port)
            {
                this.sunucu = sunucu;
                this.port = port;
            }

            /// <summary>
            /// Dinlemeyi baþlatýr
            /// </summary>
            /// <returns>iþlemin baþarý durumu</returns>
            public bool Baslat()
            {
                if (baglan())
                {
                    calisiyor = true;
                    thread = new Thread(new ThreadStart(tDinle));
                    thread.Start();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            /// <summary>
            /// dinlemeyi durdurur
            /// </summary>
            /// <returns>iþlemin baþarý durumu</returns>
            public bool Durdur()
            {
                try
                {
                    calisiyor = false;
                    baglantiyiKes();
                    thread.Join();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Port dinleme iþlemini baþlatýr ve baðlantýyý açýk hale getirir
            /// </summary>
            /// <returns>Ýþlemin baþarý durumu</returns>
            private bool baglan()
            {
                try
                {
                    dinleyiciSoket = new TcpListener(System.Net.IPAddress.Any, port);
                    dinleyiciSoket.Start();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Port dinleme iþlemini kapatýr
            /// </summary>
            /// <returns>Ýþlemin baþarý durumu</returns>
            private bool baglantiyiKes()
            {
                try
                {
                    dinleyiciSoket.Stop();
                    dinleyiciSoket = null;
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            /// <summary>
            /// Ayrý bir thread olarak çalýþýp gelen soket baðlantýlarýný kabul ederek 
            /// ASMESSunucusu modülüne ileten fonksyon.
            /// </summary>
            public void tDinle()
            {
                Socket istemciSoketi;
                while (calisiyor)
                {
                    try
                    {
                        istemciSoketi = dinleyiciSoket.AcceptSocket();
                        if (istemciSoketi.Connected)
                        {
                            try { sunucu.yeniIstemciSoketiBaglandi(istemciSoketi); }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception)
                    {
                        if (calisiyor)
                        {
                            //baðlantýyý sýfýrla
                            baglantiyiKes();
                            //1 saniye bekle
                            try { Thread.Sleep(1000); }
                            catch (Exception) { }
                            //yeniden baðlantý kur
                            baglan();
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Sunucuya baðlantý kuran bir istemciyi temsil eder.
        /// </summary>
        private class Istemci : IIstemci
        {
            // Sabitler -------------------------------------------------------

            private const byte BASLANGIC_BYTE = (byte)60;
            private const byte BITIS_BYTE = (byte)62;

            // Public Özellikler ----------------------------------------------

            /// <summary>
            /// Ýstemciyi temsil eden tekil ID deðeri
            /// </summary>
            public long IstemciID
            {
                get { return istemciID; }
            }

            /// <summary>
            /// Ýstemci ile baðlantýnýn doðru þekilde kurulu olup olmadýðýný verir. True ise mesaj yollanýp alýnabilir.
            /// </summary>
            public bool BaglantiVar
            {
                get { return calisiyor; }
            }

            // OLAYLAR --------------------------------------------------------

            /// <summary>
            /// Sunucu ile olan baðlantý kapandýðýnda tetiklenir
            /// </summary>
            public event dgBaglantiKapatildi BaglantiKapatildi;
            /// <summary>
            /// Sunucudan yeni bir mesaj alýndýðýnda tetiklenir
            /// </summary>
            public event dgYeniMesajAlindi YeniMesajAlindi;

            // Private Alanlar ------------------------------------------------

            /// <summary>
            /// Ýstemci ile iletiþimde kullanýlan soket baðlantýsý
            /// </summary>
            private Socket soket;
            /// <summary>
            /// Sunucuya referans
            /// </summary>
            private ASMESSunucusu sunucu;
            /// <summary>
            /// Ýstemciyi temsil eden tekil ID deðeri
            /// </summary>
            private long istemciID;
            /// <summary>
            /// Veri transfer etmek için kullanýlan akýþ nesnesi
            /// </summary>
            private NetworkStream agAkisi;
            /// <summary>
            /// Veri transfer etmek için kullanýlan akýþ nesnesi
            /// </summary>
            private BinaryReader binaryOkuyucu;
            /// <summary>
            /// Veri transfer etmek için kullanýlan akýþ nesnesi
            /// </summary>
            private BinaryWriter binaryYazici;
            /// <summary>
            /// Ýstemci ile iletiþim devam ediyorsa true, aksi halde false
            /// </summary>
            private volatile bool calisiyor = false;
            /// <summary>
            /// Çalýþan thread'e referans
            /// </summary>
            private Thread thread;

            // Public Fonksyonlar ---------------------------------------------

            /// <summary>
            /// Bir istemci nesnesi oluþturur
            /// </summary>
            /// <param name="sunucu">Sunucuya referans</param>
            /// <param name="istemciSoketi">Ýstemci ile iletiþimde kullanýlan soket baðlantýsý</param>
            /// <param name="istemciID">Ýstemciyi temsil eden tekil ID deðeri</param>
            public Istemci(ASMESSunucusu sunucu, Socket istemciSoketi, long istemciID)
            {
                this.sunucu = sunucu;
                this.soket = istemciSoketi;
                this.istemciID = istemciID;
            }

            /// <summary>
            /// Ýstemci ile mesaj alýþveriþini baþlatýr
            /// </summary>
            /// <returns></returns>
            public bool Baslat()
            {
                try
                {
                    agAkisi = new NetworkStream(soket);
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
            /// Ýstemci ile mesaj alýþveriþini durdurur
            /// </summary>
            public void Durdur()
            {
                try
                {
                    calisiyor = false;
                    soket.Close();
                    thread.Abort();
                    thread.Join();
                }
                catch (Exception)
                {

                }
            }

            /// <summary>
            /// Ýstemci ile olan baðlantýyý keser
            /// </summary>
            public void BaglantiyiKapat()
            {
                this.Durdur();
            }

            /// <summary>
            /// Ýstemciye bir mesaj yollamak içindir
            /// </summary>
            /// <param name="mesaj">Yollanacak mesaj</param>
            /// <returns>Ýþlemin baþarý durumu</returns>
            public bool MesajYolla(string mesaj)
            {
                try
                {
                    byte[] bMesaj = System.Text.Encoding.ASCII.GetBytes(mesaj);
                    byte[] b = new byte[bMesaj.Length + 2];
                    Array.Copy(bMesaj, 0, b, 1, bMesaj.Length);
                    b[0] = BASLANGIC_BYTE;
                    b[b.Length - 1] = BITIS_BYTE;
                    binaryYazici.Write(b);
                    agAkisi.Flush();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            // PRIVATE FONKSYONLAR ////////////////////////////////////////////

            /// <summary>
            /// Ýstemciden mesajlarý dinleyen fonksyon
            /// </summary>
            private void tCalis()
            {
                //Her döngüde bir mesaj okunup sunucuya iletilir
                while (calisiyor)
                {
                    try
                    {
                        //Baþlangýç Byte'ýný oku
                        byte b = binaryOkuyucu.ReadByte();
                        if (b != BASLANGIC_BYTE)
                        {
                            //Baþlangýç byte'ý deðil, bu karakteri atla!
                            continue;
                        }
                        //Mesajý oku
                        List<byte> bList = new List<byte>();
                        while ((b = binaryOkuyucu.ReadByte()) != BITIS_BYTE)
                        {
                            bList.Add(b);
                        }
                        string mesaj = System.Text.Encoding.ASCII.GetString(bList.ToArray());
                        //Okunan paketi sunucuya ilet
                        sunucu.yeniIstemciMesajiAlindi(this, mesaj);
                        yeniMesajAlindiTetikle(mesaj);
                    }
                    catch (Exception)
                    {
                        //Hata oluþtu, baðlantýyý kes!
                        break;
                    }
                }
                //Döngüden çýkýldýðýna göre istemciyle baðlantý kapatýlmýþ ya da bir hata oluþmuþ demektir
                calisiyor = false;
                try
                {
                    if (soket.Connected)
                    {
                        soket.Close();
                    }
                }
                catch (Exception)
                {

                }
                sunucu.istemciBaglantisiKapatildi(this);
                baglantiKapatildiTetikle();
            }

            // Olaylarý tetikleyen iç fonksyonlar -----------------------------

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
}
