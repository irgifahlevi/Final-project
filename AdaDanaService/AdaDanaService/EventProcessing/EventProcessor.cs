using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AdaDanaService.Data;
using AdaDanaService.Dtos;
using AutoMapper;

namespace AdaDanaService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
        {
            _scopeFactory = serviceScopeFactory;
            _mapper = mapper;
        }


        public void ProccessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.CashOutPublished:
                    cashOut(message);
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
                case "CashoutWallet_NewPublished":
                    Console.WriteLine("--> CashoutWallet_NewPublished Event Detected");
                    return EventType.CashOutPublished;
                default:
                    Console.WriteLine("--> Could not determine the event type");
                    return EventType.Undetermined;
            }
        }


        private void cashOut(string cashOutPublishMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IWalletService>();
                var cashOutPubishedDto = JsonSerializer.Deserialize<CashOutPublisedDto>(cashOutPublishMessage);
                try
                {
                    var cashToSaldo = new CashToSaldoDto();
                    cashToSaldo.Username = cashOutPubishedDto.Username;
                    cashToSaldo.Saldo = cashOutPubishedDto.Cash;
                    var cashOutWallet = _mapper.Map<CashOutDto>(cashToSaldo);


                    var user = repo.ExternalUserExists(cashOutWallet.Username);
                    if (user == null)
                    {
                        Console.WriteLine($"--> User with username {cashOutWallet.Username} does not exist");
                        return;
                    }

                    Console.WriteLine($"-->Username BukaToko : {cashOutWallet.Username} == Username AdaDana : {user.Username}");

                    var wallet = repo.GetWalletByUserId(user.Id);
                    if (wallet == null)
                    {
                        Console.WriteLine($"--> Wallet for user {cashOutWallet.Username} does not exist");
                        return;
                    }

                    wallet.Saldo -= cashOutWallet.Saldo;
                    repo.WalletUpdate(wallet);
                    Console.WriteLine($"--> Wallet succes {wallet.Saldo} - {cashOutWallet.Saldo}");


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add Product to DB: {ex.Message}");
                }
            }
        }

        enum EventType
        {
            CashOutPublished,
            Undetermined
        }
    }
}