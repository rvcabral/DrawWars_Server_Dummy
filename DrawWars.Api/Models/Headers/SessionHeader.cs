namespace DrawWars.Api.Models.Headers
{
    public class SessionHeader
    {
        public string UserId { get; set; }

        public byte[] IV { get; set; }
    }
}
