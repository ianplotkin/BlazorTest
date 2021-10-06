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
        //public Units DefaultUnitVal { get; set; }

        public Grocery Clone()
        {
            return new Grocery
            {
                Id = Id,
                Name = Name,
                CategoryId = CategoryId,
                DefaultAmount = DefaultAmount,
                DefaultUnit = DefaultUnit,
                //DefaultUnitVal = DefaultUnitVal
            };
        }
    }
}
