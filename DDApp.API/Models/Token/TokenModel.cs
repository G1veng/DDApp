namespace DDApp.API.Models
{
    public class TokenModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public TokenModel (string accesToken, string refreshToken)
        {
            AccessToken = accesToken;
            RefreshToken = refreshToken;
        }
    }
}
