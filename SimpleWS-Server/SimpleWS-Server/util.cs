namespace SimpleWS_Server
{
    public static class util
    {
        static int positionId = 1000;
        static util()
        {
            
        }
        public static Position RandomPosition(string ticker)
        {
            Random rnd = new Random();

            String str = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int size = 9;
            String randomstring = "";

            for (int i = 0; i < size; i++)
            {

                // Selecting a index randomly 
                int x = rnd.Next(str.Length);

                // Appending the character at the  
                // index to the random alphanumeric string. 
                randomstring = randomstring + str[x];
            }

            Position pos = new Position();
            pos.issuer = ticker + "-" + randomstring;
            pos.ticker = ticker;
            pos.cusip = randomstring;
            pos.quantity = rnd.Next(6000, 8000);
            pos.positionId = positionId++;

            return pos;
        }
    }
}
