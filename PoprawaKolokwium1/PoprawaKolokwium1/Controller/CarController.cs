using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using PoprawaKolokwium1.Properties;

namespace PoprawaKolokwium1;

[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;

    public CarController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpGet("clients/{id}")]
    public async Task<IActionResult> GetClientInfo(int id)
    {
        if (!await _carService.DoesClientExists(id))
            return NotFound($"Client with given ID - {id} doesn't exist");

        var client = await _carService.GetCLientData(id);

        return Ok(client);
    }

    [HttpPost]
    public async Task<IActionResult> AddClientAndCar(CarDTO carDto)
    {
        if (!await _carService.DoesCarExists(carDto.Id))
            return BadRequest($"Client with given ID - {carDto.Id} doesn't exist");

        var idCar = await _carService.AddCar(carDto.Id, carDto.DateFrom, carDto.DateTo);
        var pricePerDay = await _carService.GetPriceCarPerDay(carDto.Id);
        /*var totalPrice = await (CarDTO.DateFrom - DateTime)*/
        ClientInCarDTO clientInCarDto = new ClientInCarDTO();
        clientInCarDto.FirstName = clientInCarDto.FirstName;
        clientInCarDto.LastName = clientInCarDto.LastName;
        clientInCarDto.Address = clientInCarDto.Address;
        /*carDto.Client = new List<ClientDTO>(clientInCarDto.)*/

        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            /*var id = await _carService.AddClientAndCar(carDto.Id,
            {
                Id = carDto.Id,
                DateFrom = carDto.DateFrom,
                DateTo = carDto.DateTo
            });*/
            /*foreach (var res in carDto.ClientInCar)
            {
                /*await _carService.AddCar(res);#1#
            }*/

            scope.Complete();
            return Ok();
        }
        
        
    }

}