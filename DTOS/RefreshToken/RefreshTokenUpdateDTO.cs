namespace Coding.DTOS.RefreshToken
{
    public class RefreshTokenUpdateDTO
    {
        public string? Token { get; set; }
        public DateTime? ExpireDate { get; set; }
        public bool? IsRevoked { get; set; }
        public Guid? UserId { get; set; }
    }
}
