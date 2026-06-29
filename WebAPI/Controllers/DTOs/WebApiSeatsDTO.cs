using System.Collections.Generic;

namespace WebAPI.Controllers.DTOs
{
    public class WebApiSeatsDTO
    {
        public int VotingNumber { get; set; }

        public List<WebApiSeatDTO> Seats { get; set; } = new List<WebApiSeatDTO>();
    }
}
