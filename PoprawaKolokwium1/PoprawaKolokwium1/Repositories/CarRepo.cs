using Microsoft.Data.SqlClient;
using PoprawaKolokwium1.Properties;

namespace PoprawaKolokwium1;

public class CarService : ICarService
{

    private readonly IConfiguration _configuration;

    public CarService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> DoesClientExists(int id)
    {
        var query = "SELECT 1 FROM Clients WHERE ID = @ID";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
       
        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;

    }
    
    
    public async Task<ClientDTO> GetCLientData(int id)
    {
        var query = @"SELECT Clients.ID AS ClientID,
                       Clients.FirstName AS FirstName,
                       Clients.LastName AS LastName,
                       Clients.Address AS Address,
                       Car.VIN AS VIN,
                       Colors.Name AS Color,
                       Models.Name AS Model,
                       Car_Rentals.DateFrom AS DateFrom,
                       Car_Rentals.DateTo AS DateTo,
                       Car_Rentals.TotalPrice AS TotalPrice,
                       From Clients
                       JOIN Car_Rentals ON Car_Rentals.ID = ClientID
                       JOIN Cars ON Cars.ID = CarID
                       JOIN Colors ON Colors.ID = ColorID
                       JOIN Models ON Models.ID = ModelID 
                       WHERE Clients.ID = @ID";
        
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
	    
        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        var clientIDOrdinal = reader.GetOrdinal("ClientID");
        var firstNameOrdinal = reader.GetOrdinal("FirstName");
        var lastNameOrdinal = reader.GetOrdinal("LastName");
        var addressOrdinal = reader.GetOrdinal("Address");
        var vinOrdinal = reader.GetOrdinal("VIN");
        var colorOrdinal = reader.GetOrdinal("Color");
        var modelOrdinal = reader.GetOrdinal("Model");
        var dateFromOrdinal = reader.GetOrdinal("DateFrom");
        var dateToOrdinal = reader.GetOrdinal("DateTo");
        var totalPriceOrdinal = reader.GetOrdinal("TotalPrice");

        ClientDTO clientDto = null;


        while (await reader.ReadAsync())
        {
            if (clientDto is not null)
            {
                clientDto.Rentals.Add(new RentalsDTO()
                {
                    VIN = reader.GetString(vinOrdinal),
                    Color = reader.GetString(colorOrdinal),
                    Model = reader.GetString(modelOrdinal),
                    DateFrom = reader.GetDateTime(dateFromOrdinal),
                    DateTo = reader.GetDateTime(dateToOrdinal),
                    TotalPrice = reader.GetInt32(totalPriceOrdinal)
                });
            }
            else
            {
                clientDto = new ClientDTO()
                {
                    Id = reader.GetInt32(clientIDOrdinal),
                    FirstName = reader.GetString(firstNameOrdinal),
                    LastName = reader.GetString(lastNameOrdinal),
                    Address = reader.GetString(addressOrdinal),
                    Rentals = new List<RentalsDTO>()
                    {
                        new RentalsDTO()
                        {
                            VIN = reader.GetString(vinOrdinal),
                            Color = reader.GetString(colorOrdinal),
                            Model = reader.GetString(modelOrdinal),
                            DateFrom = reader.GetDateTime(dateFromOrdinal),
                            DateTo = reader.GetDateTime(dateToOrdinal),
                            TotalPrice = reader.GetInt32(totalPriceOrdinal)
                        }

                    }
                };
            }
        }
        if (clientDto is null) throw new Exception();
        
        return clientDto;
    }

    public async Task<bool> DoesCarExists(int id)
    {
        var query = "SELECT 1 FROM Cars WHERE ID = @ID";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
       
        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return res is not null;

    }

    public async Task<decimal> GetPriceCarPerDay(int id)
    {
        var query = "SELECT Cars.PricePerDay FROM Cars WHERE ID = @ID";
        
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);
       
        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        return (decimal) res;

    }

    public async Task<int> AddClient(string firstName,string lastName,string address)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();
        command.CommandText = "INSERT INTO Clients VALUES (@firstName,@lastName,@address);" +
                              "SELECT @@IDENTITY AS ID";

        command.Parameters.AddWithValue("@firstName", firstName);
        command.Parameters.AddWithValue("@lastName", lastName);
        command.Parameters.AddWithValue("@address", address);

        var id = Convert.ToInt32(await command.ExecuteScalarAsync());
        return id;
    }

    public async Task<int> AddCar(int carId,DateTime dateFrom,DateTime dateTo)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();
        command.CommandText = "INSERT INTO Car_Rentals VALUES (@carId,@dateFrom,@dateTo);" +
                              "SELECT @@IDENTITY AS ID";

        command.Parameters.AddWithValue("@carId", carId );
        command.Parameters.AddWithValue("@dateFrom", dateFrom);
        command.Parameters.AddWithValue("@dateTo", dateTo);

        var id = Convert.ToInt32(await command.ExecuteScalarAsync());
        
        return id;
    }

    public async Task AddClientAndCar(int clientId, int carId)
    {
        await using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        await using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        await connection.OpenAsync();

        command.CommandText = "INSERT INTO Car_Rentals VALUES(@clientId,@carId)";
        command.Parameters.AddWithValue("@clientId", clientId);
        command.Parameters.AddWithValue("@carId", carId);

        await command.ExecuteScalarAsync();
    }
}