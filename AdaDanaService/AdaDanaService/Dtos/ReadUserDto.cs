namespace AdaDanaService.Dtos
{
    public class ReadUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}