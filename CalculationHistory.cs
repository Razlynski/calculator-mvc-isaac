namespace CalculatorMVC.Models
{
    public class CalculationHistory
    {
        public int Id { get; set; }
        public string Expression { get; set; } = "";
        public double Result { get; set; }
        public DateTime CreatedAt { get; set; }
        public string WindowId { get; set; } = "";
    }
}


