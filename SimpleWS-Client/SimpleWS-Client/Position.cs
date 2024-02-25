using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWS_Client
{
    [Serializable]
    public class Position
    {
        public int positionId { get; set; }
        public double quantity { get; set; }

        public string cusip { get; set; }

        public string issuer { get; set; }

        public string ticker { get; set; }
    }
}
