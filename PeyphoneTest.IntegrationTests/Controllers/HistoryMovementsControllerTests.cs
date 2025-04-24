using PeyphoneTest.IntegrationTests.Response;
using PeyphoneTest.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace PeyphoneTest.IntegrationTests.Controllers
{
    public class HistoryMovementsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public HistoryMovementsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("TestAuth");
        }

        [Fact]
        public async Task GetHistoryById_ShouldReturnHistory_WhenExists()
        {
            // Crear 2 billeteras
            var fromDto = new WalletDto { DocumentId = "1111111111", Name = "Desde", Balance = 100 };
            var toDto = new WalletDto { DocumentId = "2222222222", Name = "Hasta", Balance = 0 };

            var fromRes = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", fromDto);
            var toRes = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", toDto);

            var fromWallet = await fromRes.Content.ReadFromJsonAsync<CreateWalletResponse>();
            var toWallet = await toRes.Content.ReadFromJsonAsync<CreateWalletResponse>();

            // Hacer transferencia para generar historial
            var transferDto = new TransferDto
            {
                FromWalletId = fromWallet.WalletId,
                ToWalletId = toWallet.WalletId,
                Amount = 50,
                ToWalletName = "Hasta"
            };

            await _client.PostAsJsonAsync("/api/Wallet/Transfer", transferDto);

            // Consultar historial
            var response = await _client.GetAsync($"/api/HistoryMovements/{fromWallet.WalletId}");
            response.EnsureSuccessStatusCode();

            var history = await response.Content.ReadFromJsonAsync<List<HistoryMovementDto>>();

            Assert.NotEmpty(history);
            Assert.Contains(history, h => h.Type == "Débito");
        }


        [Fact]
        public async Task GetHistoryById_ShouldReturn404_WhenNoMovements()
        {
            // Crear billetera sin transferencias
            var dto = new WalletDto { DocumentId = "3333333333", Name = "SinMovs", Balance = 10 };
            var createResponse = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", dto);
            var wallet = await createResponse.Content.ReadFromJsonAsync<CreateWalletResponse>();

            // Consultar historial
            var response = await _client.GetAsync($"/api/HistoryMovements/{wallet.WalletId}");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("No hay movimientos registrados", content);
        }

        [Fact]
        public async Task GetHistoryById_ShouldReturn400_WhenIdIsInvalid()
        {
            var response = await _client.GetAsync("/api/HistoryMovements/0");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("ID de la billetera debe ser válido", content);
        }
    }

}


