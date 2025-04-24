using PeyphoneTest.Models.Dtos;
using System.Net.Http.Json;
using Xunit;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using PeyphoneTest.IntegrationTests.Response;

namespace PeyphoneTest.IntegrationTests.Controllers
{
    public class WalletControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public WalletControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("TestAuth");
        }

        [Fact]
        public async Task CreateWallet_ShouldReturnSuccessAndWalletId()
        {
            var dto = new WalletDto
            {
                DocumentId = "9999999999",
                Name = "Integracion Uno",
                Balance = 500
            };

            var response = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", dto);
            var rawContent = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<CreateWalletResponse>();
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.True(result.WalletId > 0);
        }


        [Fact]
        public async Task GetAllWallets_ShouldReturnWalletList()
        {
            var walletDto = new WalletDto
            {
                DocumentId = "8888888888",
                Name = "Test Wallet",
                Balance = 100
            };

            await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", walletDto);
            var response = await _client.GetAsync("/api/Wallet/GetAllWallet");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            Assert.Contains("Test Wallet", json);
        }

        [Fact]
        public async Task Transfer_ShouldTransferBalanceSuccessfully()
        {
            var fromWalletDto = new WalletDto { DocumentId = "1234567890", Name = "From", Balance = 100 };
            var toWalletDto = new WalletDto { DocumentId = "0987654321", Name = "To", Balance = 0 };

            var fromResponse = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", fromWalletDto);
            var toResponse = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", toWalletDto);

            var fromResult = await fromResponse.Content.ReadFromJsonAsync<CreateWalletResponse>();
            var toResult = await toResponse.Content.ReadFromJsonAsync<CreateWalletResponse>();

            var transferDto = new TransferDto
            {
                FromWalletId = fromResult.WalletId,
                ToWalletId = toResult.WalletId,
                Amount = 50,
                ToWalletName = "To"
            };

            var response = await _client.PostAsJsonAsync("/api/Wallet/Transfer", transferDto);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TransferResponse>();
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Transferencia completada exitosamente.", result.Message);
        }

        [Fact]
        public async Task Transfer_ShouldFail_WhenToWalletNameIsIncorrect()
        {
            var fromWalletDto = new WalletDto { DocumentId = "2222222222", Name = "A", Balance = 100 };
            var toWalletDto = new WalletDto { DocumentId = "3333333333", Name = "B", Balance = 0 };

            var fromResponse = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", fromWalletDto);
            var toResponse = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", toWalletDto);

            var fromResult = await fromResponse.Content.ReadFromJsonAsync<CreateWalletResponse>();
            var toResult = await toResponse.Content.ReadFromJsonAsync<CreateWalletResponse>();

            var transferDto = new TransferDto
            {
                FromWalletId = fromResult.WalletId,
                ToWalletId = toResult.WalletId,
                Amount = 10,
                ToWalletName = "Nombre Incorrecto" 
            };

            var response = await _client.PostAsJsonAsync("/api/Wallet/Transfer", transferDto);
            var result = await response.Content.ReadFromJsonAsync<TransferResponse>();

            Assert.False(result.Success);
            Assert.Equal("El nombre de la cuenta de destino no coincide con el ID proporcionado.", result.Message);
        }

        [Fact]
        public async Task Transfer_ShouldFail_WhenToWalletNameIsMissing()
        {
            var fromWalletDto = new WalletDto { DocumentId = "4444444444", Name = "X", Balance = 100 };
            var toWalletDto = new WalletDto { DocumentId = "5555555555", Name = "Y", Balance = 0 };

            var fromResponse = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", fromWalletDto);
            var toResponse = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", toWalletDto);

            var fromResult = await fromResponse.Content.ReadFromJsonAsync<CreateWalletResponse>();
            var toResult = await toResponse.Content.ReadFromJsonAsync<CreateWalletResponse>();

            var transferDto = new TransferDto
            {
                FromWalletId = fromResult.WalletId,
                ToWalletId = toResult.WalletId,
                Amount = 10,
                ToWalletName = "" 
            };

            var response = await _client.PostAsJsonAsync("/api/Wallet/Transfer", transferDto);
            var result = await response.Content.ReadFromJsonAsync<TransferResponse>();

            Assert.False(result.Success);
            Assert.Equal("Debe proporcionar el nombre de la cuenta de destino.", result.Message);
        }

        [Fact]
        public async Task UpdateWallet_ShouldUpdateSuccessfully()
        {
            var createDto = new WalletDto
            {
                DocumentId = "1111111111",
                Name = "Original",
                Balance = 100
            };

            var createResponse = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", createDto);
            var created = await createResponse.Content.ReadFromJsonAsync<CreateWalletResponse>();

            var updateDto = new WalletDto
            {
                DocumentId = "1111111111", // puede mantenerse igual
                Name = "Actualizado",
                Balance = 100 // no se usa pero requerido por DTO
            };

            var updateResponse = await _client.PutAsJsonAsync($"/api/Wallet/UpdateWallet/{created.WalletId}", updateDto);
            updateResponse.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync($"/api/Wallet/{created.WalletId}");
            var updatedWallet = await getResponse.Content.ReadAsStringAsync();

            Assert.Contains("Actualizado", updatedWallet);
        }

        [Fact]
        public async Task DeleteWallet_ShouldDeleteSuccessfully()
        {
            var createDto = new WalletDto
            {
                DocumentId = "2222222222",
                Name = "A Eliminar",
                Balance = 100
            };

            var createResponse = await _client.PostAsJsonAsync("/api/Wallet/CreateWallet", createDto);
            var created = await createResponse.Content.ReadFromJsonAsync<CreateWalletResponse>();

            var deleteResponse = await _client.DeleteAsync($"/api/Wallet/DeleteWallet/{created.WalletId}");
            deleteResponse.EnsureSuccessStatusCode();

            var getResponse = await _client.GetAsync($"/api/Wallet/{created.WalletId}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task UpdateWallet_ShouldFail_WhenWalletDoesNotExist()
        {
            var updateDto = new WalletDto
            {
                DocumentId = "3333333333",
                Name = "No Existe",
                Balance = 0
            };

            var response = await _client.PutAsJsonAsync("/api/Wallet/UpdateWallet/99999", updateDto);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteWallet_ShouldFail_WhenWalletDoesNotExist()
        {
            var response = await _client.DeleteAsync("/api/Wallet/DeleteWallet/99999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


    }
}


