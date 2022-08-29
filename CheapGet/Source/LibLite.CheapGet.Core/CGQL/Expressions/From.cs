namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class From : Expression
    {
        public Text Text { get; set; }

        public From(Text text)
        {
            Text = text;
        }

        public override bool Equals(object obj)
        {
            return obj is From from &&
                   EqualityComparer<Text>.Default.Equals(Text, from.Text);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text);
        }
    }
}
