using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Filters;
using WebAPI.Data;
using WebAPI.StorageClient;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Controller for retrieving agenda point sub-items.
    /// </summary>
    [ApiController]
    [Route("agendapoint")]
    [TypeFilter(typeof(WebAPIExceptionFilter))]
    public class AgendaPointSubItemsControllerController: ControllerBase
    {
        private readonly IAgendaSubItemsProvider _agendaSubItemsProvider;
        private readonly ILogger<AgendaPointSubItemsControllerController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AgendaPointSubItemsControllerController"/> class.
        /// </summary>
        /// <param name="agendaSubItemsProvider">The agenda sub-items data provider.</param>
        /// <param name="logger">The logger instance.</param>
        public AgendaPointSubItemsControllerController(
            IAgendaSubItemsProvider agendaSubItemsProvider,
            ILogger<AgendaPointSubItemsControllerController> logger)
        {
            _agendaSubItemsProvider = agendaSubItemsProvider;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves sub-items for a specific agenda point in a meeting.
        /// </summary>
        /// <param name="meetingId">The meeting identifier.</param>
        /// <param name="agendaPoint">The agenda point number.</param>
        /// <returns>The sub-items or 204 No Content if none found.</returns>
        [HttpGet]
        [Route("{meetingId}/{agendaPoint}")]
        public async Task<IActionResult> GetSubItems(string meetingId, int agendaPoint)
        {
            _logger.LogInformation("Executing GetSubItems() {0} {1}", meetingId, agendaPoint);

            var items = await _agendaSubItemsProvider.GetAgendaPointSubItems(meetingId, agendaPoint);
            if (items == null)
            {
                return NoContent();
            }

            return Ok(items);
        }
    }
};
