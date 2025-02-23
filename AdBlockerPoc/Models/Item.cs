namespace AdBlockerPoc.Models
{
    public class Item
    {
        public DateTime CreationDate { get; set; }
        public string Content { get; set; }
        public bool IsBlocked { get; set; }

        public Item(string content, bool isBlocked)
        {
            Content = content;
            IsBlocked = isBlocked;
            CreationDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{CreationDate} ◾ {Content} {(IsBlocked ? " ◾ BLOCKED" : " ◾ ALLOWED")}";
        }
    }
}
