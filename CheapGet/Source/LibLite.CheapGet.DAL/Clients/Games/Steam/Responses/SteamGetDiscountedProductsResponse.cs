namespace LibLite.CheapGet.DAL.Clients.Games.Steam.Responses
{
    public class SteamGetDiscountedProductsResponse
    {
        public int success { get; set; }
        public string results_html { get; set; }
        public int total_count { get; set; }
        public int start { get; set; }
    }
}
