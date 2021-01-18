namespace DataLayer
{
    public class UserTask
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public TaskStatus TaskStatus { get; set; }
        
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }

    public enum TaskStatus
    {
        TODO, COMPLETED
    }
}
