// See https://aka.ms/new-console-template for more information
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;
using SL.Application.Services.Mappers.Interfaces;
using SL.Domain.Models;
using SL.StartAppPresentation.Extentions;


ILog log = LogManager.GetLogger(typeof(Program));
XmlConfigurator.Configure();

try
{
    var serviceProvider = new ServiceCollection().AddServices().BuildServiceProvider(); 

    var mapHandler = serviceProvider.GetService<IMapHandler>();


   var reservation = new ReservationMdl
   {
       Id = 123,
       Date = DateTime.Now,
       CustomerName = "Saeed Lashani"
   };

    //Object GoogleReservation = new
    //{
    //    GoogleReservationId = "123",
    //    DateOfReservation = "2024-11-14",
    //    GuestName = "Saeed Lashani"
    //};



    object googleReservation = await mapHandler.Map(reservation, "Models.ReservationMdl", "Google.Reservation");
    //object googleReservation = mapHandler.Map(GoogleReservation, "Google.Reservation", "Models.ReservationMdl");

    var googleReservationType = googleReservation.GetType();
    Console.WriteLine(googleReservationType.GetProperty("GoogleReservationId").GetValue(googleReservation));  // Outputs: 123
    Console.WriteLine(googleReservationType.GetProperty("DateOfReservation").GetValue(googleReservation));    // Outputs: current date in yyyy-MM-dd format
    Console.WriteLine(googleReservationType.GetProperty("GuestName").GetValue(googleReservation));           // Outputs: Saeed Lashani
}
catch (Exception ex)
{
    log.Error(ex.Message);
}
