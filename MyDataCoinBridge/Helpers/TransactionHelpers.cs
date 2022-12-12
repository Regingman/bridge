namespace MyDataCoinBridge.Helpers
{
    public static class TransactionHelpers
    {
        public static string TransactionType(int type)
        {
            switch (type)
            {
                case 0: return "Views";
                case 1: return "Clicks";
                case 2: return "Conversions";
                case 3: return "Something";
                case 4: return "";
                case 5: return "";
                case 6: return "";
                case 7: return "";
                case 8: return "";
                case 9: return "";
                default: return "";
            }
        }

        public static int TransactionTypeToInt(string type)
        {
            switch (type)
            {
                case "Views": return 0;
                case "Clicks": return 1;
                case "Conversions": return 2;
                case "Something": return 3;
                default: return -1;
            }
        }

    }
}
