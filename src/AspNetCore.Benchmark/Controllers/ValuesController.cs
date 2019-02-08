using MicroBenchmarks.Serializers;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Benchmark.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private static readonly MyEventsListerViewModel ViewModel = DataGenerator.Generate<MyEventsListerViewModel>();

        [HttpGet]
        public ActionResult<MyEventsListerViewModel> Get()
        {
            return this.Ok(ViewModel);
        }

        [HttpPost]
        public ActionResult Post(MyEventsListerViewModel viewModel)
        {
            return this.Ok();
        }
    }
}
