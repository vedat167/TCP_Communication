using System;
using System.Collections.Generic;
using System.Text;

namespace HIK.ASMES.SunucuTarafi
{
    // Delegateler ////////////////////////////////////////////////////////////

    public delegate void dgYeniIstemciBaglandi(IstemciBaglantiArgumanlari e);
    public delegate void dgIstemciBaglantisiKapatildi(IstemciBaglantiArgumanlari e);
    public delegate void dgIstemcidenYeniMesajAlindi(IstemcidenMesajAlmaArgumanlari e);

    // Delegate'ler için parametre sýnýflarý //////////////////////////////////

    public class IstemciBaglantiArgumanlari : EventArgs
    {
        public IIstemci Istemci
        {
            get { return istemci; }
        }
        private IIstemci istemci;

        public IstemciBaglantiArgumanlari(IIstemci istemci)
        {            
            this.istemci = istemci;
        }
    }

    public class IstemcidenMesajAlmaArgumanlari : EventArgs
    {
        public IIstemci Istemci
        {
            get { return istemci; }
        }
        private IIstemci istemci;

        public string Mesaj
        {
            get { return mesaj; }
            set { mesaj = value; }
        }
        private string mesaj;

        public IstemcidenMesajAlmaArgumanlari(IIstemci istemci, string mesaj)
        {
            this.istemci = istemci;
            this.mesaj = mesaj;
        }
    }
}
