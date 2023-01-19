using System;
using System.Collections.Generic;
using System.Text;

namespace HIK.ASMES
{
    // Delegateler ////////////////////////////////////////////////////////////

    public delegate void dgBaglantiKapatildi();
    public delegate void dgYeniMesajAlindi(MesajAlmaArgumanlari e);

    // Delegate'ler i�in parametre s�n�flar� //////////////////////////////////

    public class MesajAlmaArgumanlari : EventArgs
    {
        public string Mesaj
        {
            get { return mesaj; }
            set { mesaj = value; }
        }
        private string mesaj;

        public MesajAlmaArgumanlari(string mesaj)
        {
            this.mesaj = mesaj;
        }
    }
}
