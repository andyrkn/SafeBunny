using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SafeBunny.Core.Message;
using SafeBunny.Core.Publishing;
using SafeBunny.Domain;

namespace SafeBunny.Client.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SafeBunnyController : ControllerBase
    {
        private readonly ISafeBunnyPublisher publisher;

        public SafeBunnyController(ISafeBunnyPublisher publisher)
        {
            this.publisher = publisher;
        }

        [HttpPost]
        public async Task<IActionResult> Trigger([FromBody] NewOrderModel model)
        {
            await publisher.PublishAsync(model, new MessageProperties()).ConfigureAwait(false);
            return Created(string.Empty, null);
        }
    }
}