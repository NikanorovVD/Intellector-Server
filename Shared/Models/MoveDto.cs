namespace Shared.Models
{
    public class MoveDto
    {
        public Move Move { get; set; }
        public string UserId {  get; set; }
        public DateTime MadeAt { get; set; }
    }
}
