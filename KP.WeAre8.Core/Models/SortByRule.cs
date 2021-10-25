namespace KP.WeAre8.Core.Models
{
    public class SortByRule
    {
        public string SortKey { get; set; }
        public bool SortOrderDesc { get; set; } = false;   // fasle -> ASC || true -> DESC 
    }
}
