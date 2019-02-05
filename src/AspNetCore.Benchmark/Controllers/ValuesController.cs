using Microsoft.AspNetCore.Mvc;
using MicroBenchmarks.Serializers;

namespace AspNetCore.Benchmark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static readonly MyEventsListerViewModel ViewModel = DataGenerator.Generate<MyEventsListerViewModel>();

        // GET api/values
        [HttpGet]
        public ActionResult<MyEventsListerViewModel> Get()
        {
            return this.Ok(ViewModel);
        }
    }
}
