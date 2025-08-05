namespace ModernizationPoC.Shared
{
    public class ModernizationToggle
    {
        public int Id { get; set; }
        public bool AboutPage { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, AboutPage: {AboutPage}";
        }
    }
}