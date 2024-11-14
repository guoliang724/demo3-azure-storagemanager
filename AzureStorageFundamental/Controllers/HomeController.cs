using AzureStorageFundamental.Models;
using AzureStorageFundamental.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureStorageFundamental.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContainerServices _containerService;
        private readonly IBlobServices _blobService;

        public HomeController(IContainerServices containerService, IBlobServices blobService)
        {
            _containerService = containerService;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _containerService.GetAllContainerAndBlobs());
        }

        public async Task<IActionResult> Images()
        {
            return View(await _blobService.GetAllBlobsWithUri("privatecontainer"));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
