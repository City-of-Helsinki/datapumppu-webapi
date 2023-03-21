using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("agendapoint")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class AgendaPointSubItemsControllerController: ControllerBase
    {
        private readonly IAgendaSubItemsProvider _agendaSubItemsProvider;
        private readonly ILogger<MeetingController> _logger;

        public AgendaPointSubItemsControllerController(
            IAgendaSubItemsProvider agendaSubItemsProvider,
            ILogger<MeetingController> logger)
        {
            _agendaSubItemsProvider = agendaSubItemsProvider;
            _logger = logger;
        }

        [HttpGet]
        [Route("{meetingId}/{agendaPoint}")]
        public async Task<IActionResult> GetSubItems(string meetingId, int agendaPoint)
        {
            _logger.LogInformation("Executing GetMeeting()");

            var items = await _agendaSubItemsProvider.GetAgendaPointSubItems(meetingId, agendaPoint);
            if (items == null)
            {
                return NoContent();
            }

            return Ok(items);
        }
    }
};
