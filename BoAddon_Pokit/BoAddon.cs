using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BoAddon_Pokit
{
    public class BoAddon : SAPBoAddon.BoAddon.BoAddonBase
    {
        public static void Main()
        {
            BoAddon BoAddon = new BoAddon();
        }

        protected override SAPBoAddon.B1Connections.ConnectionType ConnectionType
        {
            get { return SAPBoAddon.B1Connections.ConnectionType.SSO; }
        }

        protected override string addOnIdentifier
        {
            get { return ""; }
        }
    }
}
