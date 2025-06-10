namespace WebApplication1.Models
{
    public class RefreshTokens
    {

        public int Id { get; set; }

        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool Revoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Email { get; set; } = string.Empty; 
        public string Password { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;    
    }
}
