using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using BukaToko.Data;
using BukaToko.DTOS;
using BukaToko.Models;
namespace BukaToko.Event
{
    public class EventProccessor : IEventProccessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProccessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _scopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }
        public void ProccessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.TopupWalletPublished:
                    topupWallet(message);
                    break;
                default:
                    break;
            }
        }
        private EventType DetermineEvent(string notificationMessage)
        {
            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType.Event)
            {
                case "TopupWallet_NewPublished":
                    Console.WriteLine("--> TopupWallet_NewPublished Event Detected");
                    return EventType.TopupWalletPublished;
                case "Wallet_Published":
                    Console.WriteLine("--> Wallet_NewPublished Event Detected");
                    return EventType.WalletPublishedDto;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }
        
        private void topupWallet(string topupWalletMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IWalletRepo>();
                var topUpPublishedDto = JsonSerializer.Deserialize<TopUpPublishedDto>(topupWalletMessage);
                try
                {
                    var cashToSaldoDto = new CashToSaldoDto();
                    cashToSaldoDto.Username = topUpPublishedDto.Username;
                    cashToSaldoDto.Cash = topUpPublishedDto.Saldo;
                    var ReadTopUpDto = _mapper.Map<ReadTopUpDto>(cashToSaldoDto);
                    if (repo.ExternalWalletExists(ReadTopUpDto.Username))
                    {
                        repo.TopUp(ReadTopUpDto.Username, ReadTopUpDto.Cash);
                        repo.SaveChanges();
                        Console.WriteLine($"--> Received Topup {ReadTopUpDto.Username}, {ReadTopUpDto.Cash} added");
                    }
                    else
                    {
                        Console.WriteLine("--> Username not found");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add TopUp to DB: {ex.Message}");
                }

            }
        }

    }
    enum EventType
    {
        TopupWalletPublished,
        WalletPublishedDto,
        Undetermined
    }
}
