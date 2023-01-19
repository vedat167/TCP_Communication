using System;
using System.Collections.Generic;
using System.Text;

namespace HIK.ASMES.SunucuTarafi
{
    /// <summary>
    /// Sunucuya baðlý olan bir istemciyi temsil eder
    /// </summary>
    public interface IIstemci
    {
        /// <summary>
        /// Ýstemciyi temsil eden tekil ID deðeri
        /// </summary>
        long IstemciID { get; }

        /// <summary>
        /// Ýstemci ile baðlantýnýn doðru þekilde kurulu olup olmadýðýný verir. True ise mesaj yollanýp alýnabilir.
        /// </summary>
        bool BaglantiVar { get; }
        
        /// <summary>
        /// Ýstemciye bir mesaj yollamak içindir
        /// </summary>
        /// <param name="mesaj">Yollanacak mesaj</param>
        /// <returns>Ýþlemin baþarý durumu</returns>
        bool MesajYolla(string mesaj);

        /// <summary>
        /// Ýstemci ile olan baðlantýyý kapatýr
        /// </summary>
        void BaglantiyiKapat();
    }
}
