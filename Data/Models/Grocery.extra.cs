namespace BlazorTest.Data.Models
{
    public enum Units
    {
        Piece,
        Lbs,
        Gallon,
        Quart
    }

    public partial class Grocery
    {
        public Units DefaultUnitVal { get; set; }
    }
}
