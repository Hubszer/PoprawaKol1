using System.Runtime.InteropServices.JavaScript;

namespace PoprawaKolokwium1.Properties;

public class CarDTO
{
    public int Id { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public ClientInCarDTO Client { get; set; }
    /*public List<ClientDTO> Client { get; set; } = new List<ClientDTO>();*/
}

public class ClientInCarDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
}