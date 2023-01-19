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
    /// Asenkron Soketli Mesajla�ma Sunucusu
    /// </summary>
    public class ASMESSunucusu
    {
        // PUBLIC �ZELL�KLER //////////////////////////////////////////////////

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
        /// Sunucuya yeni bir istemci ba�lant���nda tetiklenir.
        /// </summary>
        public event dgYeniIstemciBaglandi YeniIstemciBaglandi;
        /// <summary>
        /// Sunucuya ba�l� bir istemci ba�lant�s� kapat�ld���nda tetiklenir.
        /// </summary>
        public event dgIstemciBaglantisiKapatildi IstemciBaglantisiKapatildi;
        /// <summary>
        /// Bir istemciden yeni bir mesaj al�nd���nda tetiklenir.
        /// </summary>
        public event dgIstemcidenYeniMesajAlindi IstemcidenYeniMesajAlindi;

        // PRIVATE ALANLAR ////////////////////////////////////////////////////

        /// <summary>
        /// En son ba�lant� sa�lanan istemci ID'si
        /// </summary>
        private long sonIstemciID = 0;
        /// <summary>
        /// O anda ba�lant� kurulmu� olan istemcilerin listesi
        /// </summary>
        private SortedList<long, Istemci> istemciler;
        /// <summary>
        /// Sunucunun �al��ma durumu
        /// </summary>
        private volatile bool calisiyor;
        /// <summary>
        /// Senkronizasyonda kullan�lacak nesne
        /// </summary>
        private object objSenk = new object();
        /// <summary>
        /// Ba�lant� dinleyen nesne
        /// </summary>
        private BaglantiDinleyici baglantiDinleyici;

        // PUBLIC FONKSYONLAR /////////////////////////////////////////////////
        
        /// <summary>
        /// Bir ASMES sunucusu olu�turur
        /// </summary>
        /// <param name="port">Dinlenecek port</param>
        public ASMESSunucusu(int port)
        {
            this.port = port;
            this.istemciler = new SortedList<long, Istemci>();
            this.baglantiDinleyici = new BaglantiDinleyici(this, port);
        }

        /// <summary>
        /// Sunucunun �al��mas�n� ba�lat�r
        /// </summary>
        public void Baslat()
        {
            //E�er zaten �al���yorsa i�lem yapmadan ��k
            if (calisiyor)
            {
                return;
            }
            //Dinleyiciyi ba�lat
            if (!baglantiDinleyici.Baslat())
            {
                return;
            }
            //�al���yor bayra��n� i�aretle
            calisiyor = true;
        }

        /// <summary>
        /// Sunucunun �al��mas�n� durdurur
        /// </summary>
        public void Durdur()
        {
            //Dinleyiciyi durdur
            baglantiDinleyici.Durdur();
            //T�m istemcileri durdur
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
            //�stemcileri temizle
            istemciler.Clear();
            //�al���yor bayra��ndaki i�areti kald�r
            calisiyor = false;
        }

        /// <summary>
        /// Bir istemciye bir mesaj yollar
        /// </summary>
        /// <param name="mesaj">Yollanacak mesaj</param>
        /// <returns>��lemin ba�ar� durumu</returns>
        public bool MesajYolla(IIstemci istemci, string mesaj)
        {
            return istemci.MesajYolla(mesaj);
        }

        // PRIVATE FONKSYONLAR ////////////////////////////////////////////////

        /// <summary>
        /// Yeni bir istemci ba�lant���nda buraya g�nderilir.
        /// </summary>
        /// <param name="istemciSoketi">Yeni ba�lanan istemci soketi</param>
        private void yeniIstemciSoketiBaglandi(Socket istemciSoketi)
        {
            //Yeni ba�lanan istemciyi listeye ekle
            Istemci istemci = null;
            lock (objSenk)
            {
                istemci = new Istemci(this, istemciSoketi, ++sonIstemciID);
                istemciler.Add(istemci.IstemciID, istemci);
            }
            //�stemciyi �al��maya ba�lat
            istemci.Baslat();
            //YeniIstemciBaglandi olay�n� tetikle
            if (YeniIstemciBaglandi != null)
            {
                YeniIstemciBaglandi(new IstemciBaglantiArgumanlari(istemci));
            }
        }

        /// <summary>
        /// Bir Istemci nesnesi bir mesaj ald���nda buraya iletir
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
        /// Bir Istemci nesnesiyle ili�kili ba�lant� kapat�ld���nda, buras� �a��r�l�r
        /// </summary>
        /// <param name="istemci">Kapat�lan istemci ba�lant�s�</param>
        private void istemciBaglantisiKapatildi(Istemci istemci)
        {
            //IstemciBaglantisiKapatildi olay�n� tetikle
            if (IstemciBaglantisiKapatildi != null)
            {
                IstemciBaglantisiKapatildi(new IstemciBaglantiArgumanlari(istemci));
            }
            //Kapanan istemciyi listeden ��kar
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
        /// Ayr� bir thread olarak �al���p gelen soket ba�lant�lar�n� kabul ederek 
        /// ASMESSunucusu mod�l�ne ileten s�n�f.
        /// </summary>
        private class BaglantiDinleyici
        {
            /** Gelen ba�lant�lar�n aktar�laca�� mod�l */
            private ASMESSunucusu sunucu;
            /** Gelen ba�lant�lar� dinlemek i�in sunucu soketi */
            private TcpListener dinleyiciSoket;
            /** Dinlenen portun numaras� */
            private int port;
            /** thread'in �al��mas�n� kontrol eden bayrak */
            private volatile bool calisiyor = false;
            /** �al��an thread'e referans */
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
            /// Dinlemeyi ba�lat�r
            /// </summary>
            /// <returns>i�lemin ba�ar� durumu</returns>
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
            /// <returns>i�lemin ba�ar� durumu</returns>
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
            /// Port dinleme i�lemini ba�lat�r ve ba�lant�y� a��k hale getirir
            /// </summary>
            /// <returns>��lemin ba�ar� durumu</returns>
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
            /// Port dinleme i�lemini kapat�r
            /// </summary>
            /// <returns>��lemin ba�ar� durumu</returns>
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
            /// Ayr� bir thread olarak �al���p gelen soket ba�lant�lar�n� kabul ederek 
            /// ASMESSunucusu mod�l�ne ileten fonksyon.
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
                            //ba�lant�y� s�f�rla
                            baglantiyiKes();
                            //1 saniye bekle
                            try { Thread.Sleep(1000); }
                            catch (Exception) { }
                            //yeniden ba�lant� kur
                            baglan();
                        }
                    }
                }
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Sunucuya ba�lant� kuran bir istemciyi temsil eder.
        /// </summary>
        private class Istemci : IIstemci
        {
            // Sabitler -------------------------------------------------------

            private const byte BASLANGIC_BYTE = (byte)60;
            private const byte BITIS_BYTE = (byte)62;

            // Public �zellikler ----------------------------------------------

            /// <summary>
            /// �stemciyi temsil eden tekil ID de�eri
            /// </summary>
            public long IstemciID
            {
                get { return istemciID; }
            }

            /// <summary>
            /// �stemci ile ba�lant�n�n do�ru �ekilde kurulu olup olmad���n� verir. True ise mesaj yollan�p al�nabilir.
            /// </summary>
            public bool BaglantiVar
            {
                get { return calisiyor; }
            }

            // OLAYLAR --------------------------------------------------------

            /// <summary>
            /// Sunucu ile olan ba�lant� kapand���nda tetiklenir
            /// </summary>
            public event dgBaglantiKapatildi BaglantiKapatildi;
            /// <summary>
            /// Sunucudan yeni bir mesaj al�nd���nda tetiklenir
            /// </summary>
            public event dgYeniMesajAlindi YeniMesajAlindi;

            // Private Alanlar ------------------------------------------------

            /// <summary>
            /// �stemci ile ileti�imde kullan�lan soket ba�lant�s�
            /// </summary>
            private Socket soket;
            /// <summary>
            /// Sunucuya referans
            /// </summary>
            private ASMESSunucusu sunucu;
            /// <summary>
            /// �stemciyi temsil eden tekil ID de�eri
            /// </summary>
            private long istemciID;
            /// <summary>
            /// Veri transfer etmek i�in kullan�lan ak�� nesnesi
            /// </summary>
            private NetworkStream agAkisi;
            /// <summary>
            /// Veri transfer etmek i�in kullan�lan ak�� nesnesi
            /// </summary>
            private BinaryReader binaryOkuyucu;
            /// <summary>
            /// Veri transfer etmek i�in kullan�lan ak�� nesnesi
            /// </summary>
            private BinaryWriter binaryYazici;
            /// <summary>
            /// �stemci ile ileti�im devam ediyorsa true, aksi halde false
            /// </summary>
            private volatile bool calisiyor = false;
            /// <summary>
            /// �al��an thread'e referans
            /// </summary>
            private Thread thread;

            // Public Fonksyonlar ---------------------------------------------

            /// <summary>
            /// Bir istemci nesnesi olu�turur
            /// </summary>
            /// <param name="sunucu">Sunucuya referans</param>
            /// <param name="istemciSoketi">�stemci ile ileti�imde kullan�lan soket ba�lant�s�</param>
            /// <param name="istemciID">�stemciyi temsil eden tekil ID de�eri</param>
            public Istemci(ASMESSunucusu sunucu, Socket istemciSoketi, long istemciID)
            {
                this.sunucu = sunucu;
                this.soket = istemciSoketi;
                this.istemciID = istemciID;
            }

            /// <summary>
            /// �stemci ile mesaj al��veri�ini ba�lat�r
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
            /// �stemci ile mesaj al��veri�ini durdurur
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
            /// �stemci ile olan ba�lant�y� keser
            /// </summary>
            public void BaglantiyiKapat()
            {
                this.Durdur();
            }

            /// <summary>
            /// �stemciye bir mesaj yollamak i�indir
            /// </summary>
            /// <param name="mesaj">Yollanacak mesaj</param>
            /// <returns>��lemin ba�ar� durumu</returns>
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
            /// �stemciden mesajlar� dinleyen fonksyon
            /// </summary>
            private void tCalis()
            {
                //Her d�ng�de bir mesaj okunup sunucuya iletilir
                while (calisiyor)
                {
                    try
                    {
                        //Ba�lang�� Byte'�n� oku
                        byte b = binaryOkuyucu.ReadByte();
                        if (b != BASLANGIC_BYTE)
                        {
                            //Ba�lang�� byte'� de�il, bu karakteri atla!
                            continue;
                        }
                        //Mesaj� oku
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
                        //Hata olu�tu, ba�lant�y� kes!
                        break;
                    }
                }
                //D�ng�den ��k�ld���na g�re istemciyle ba�lant� kapat�lm�� ya da bir hata olu�mu� demektir
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

            // Olaylar� tetikleyen i� fonksyonlar -----------------------------

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
