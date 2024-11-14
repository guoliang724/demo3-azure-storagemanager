using AzureStorageFundamental.Models;
using AzureStorageFundamental.Services;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageFundamental.Controllers
{
    public class ContainerController : Controller
    {
        private readonly IContainerServices _containerServices;

        public ContainerController(IContainerServices containerServices) {
            _containerServices = containerServices;
        }
        public async Task<IActionResult> Index()
        {
            var allContainers = await _containerServices.GetAllContainer();
            return View(allContainers);
        }

        public async Task<IActionResult> Delete(string containerName)
        {
            await _containerServices.DeleteContainer(containerName);
            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Create()
        {
            return View(new Container());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Container container)
        {
            await _containerServices.CreateContainer(container.Name);
          return RedirectToAction(nameof(Index));
        }
    }
}
