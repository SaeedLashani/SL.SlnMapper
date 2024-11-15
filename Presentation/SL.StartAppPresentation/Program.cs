// See https://aka.ms/new-console-template for more information
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using SL.Application.Services;
using SL.Application.Services.Mappers.Interfaces;
using SL.Domain.Models;
using SL.StartAppPresentation.Extentions;


ILog log = LogManager.GetLogger(typeof(Program));
XmlConfigurator.Configure();

string logPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName, "Logs", "mapping-config.json");


try
{
    var serviceProvider = new ServiceCollection().AddServices().BuildServiceProvider();

    var mapHandler = serviceProvider.GetService<IMapHandler>();


    // Create a source object
    var reservation = new ReservationMdl
    {
        Id = 123,
        Date = DateTime.Now,
        CustomerName = "Saeed Lashani"
    };

    ////Object GoogleReservation = new 
    ////{
    ////    GoogleReservationId = "123",
    ////    DateOfReservation = "2024-11-14",
    ////    GuestName = "Saeed Lashani"
    ////};


    // Perform mapping
    object googleReservation = await mapHandler.Map(reservation, "Models.ReservationMdl", "Google.Reservation");
    //object modelReservation = mapHandler.Map(GoogleReservation, "Google.Reservation", "Models.ReservationMdl");

    // Access properties of dynamically created type
    var googleReservationType = googleReservation.GetType();
    Console.WriteLine(googleReservationType.GetProperty("GoogleReservationId").GetValue(googleReservation));  // Outputs: 123
    Console.WriteLine(googleReservationType.GetProperty("DateOfReservation").GetValue(googleReservation));    // Outputs: current date in yyyy-MM-dd format
    Console.WriteLine(googleReservationType.GetProperty("GuestName").GetValue(googleReservation));           // Outputs: Saeed Lashani
}
catch (Exception)
{
    log.Error("An unexpected error occurred please check the logs");
}
