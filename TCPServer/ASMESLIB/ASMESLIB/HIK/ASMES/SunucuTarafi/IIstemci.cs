using System;
using System.Collections.Generic;
using System.Text;

namespace HIK.ASMES.SunucuTarafi
{
    /// <summary>
    /// Sunucuya ba�l� olan bir istemciyi temsil eder
    /// </summary>
    public interface IIstemci
    {
        /// <summary>
        /// �stemciyi temsil eden tekil ID de�eri
        /// </summary>
        long IstemciID { get; }

        /// <summary>
        /// �stemci ile ba�lant�n�n do�ru �ekilde kurulu olup olmad���n� verir. True ise mesaj yollan�p al�nabilir.
        /// </summary>
        bool BaglantiVar { get; }
        
        /// <summary>
        /// �stemciye bir mesaj yollamak i�indir
        /// </summary>
        /// <param name="mesaj">Yollanacak mesaj</param>
        /// <returns>��lemin ba�ar� durumu</returns>
        bool MesajYolla(string mesaj);

        /// <summary>
        /// �stemci ile olan ba�lant�y� kapat�r
        /// </summary>
        void BaglantiyiKapat();
    }
}
