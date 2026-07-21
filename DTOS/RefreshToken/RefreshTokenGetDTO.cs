namespace Coding.DTOS.RefreshToken
{
    public class RefreshTokenGetDTO
    {
        public Guid Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpireDate { get; set; }
        public bool IsRevoked { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
