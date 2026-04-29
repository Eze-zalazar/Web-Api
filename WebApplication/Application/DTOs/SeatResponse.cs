namespace Application.DTOs
{
    public class SeatResponse
    {
        public Guid Id { get; set; }
        public string RowIdentifier { get; set; }
        public int SeatNumber { get; set; }
        public string Status { get; set; }
        public int SectorId { get; set; }
        public decimal Price { get; set; }
    }
}