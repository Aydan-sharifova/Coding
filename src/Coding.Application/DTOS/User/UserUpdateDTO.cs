namespace Coding.DTOS.User
{
    public class UserUpdateDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public bool? IsOnline { get; set; }
        public DateTime? LastSeen { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
