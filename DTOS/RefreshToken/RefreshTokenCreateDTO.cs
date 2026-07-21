namespace Coding.DTOS.RefreshToken
{
    public class RefreshTokenCreateDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireDate { get; set; }
        public bool IsRevoked { get; set; }
        public Guid UserId { get; set; }
    }
}
