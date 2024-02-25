namespace SimpleWS_Server
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
