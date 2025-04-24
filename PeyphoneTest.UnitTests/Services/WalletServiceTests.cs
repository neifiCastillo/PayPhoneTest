using Microsoft.EntityFrameworkCore;
using PeyphoneTest.Models.Dtos;
using PeyphoneTest.Models;
using PeyphoneTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeyphoneTest.UnitTests.Services
{
    public class WalletServiceTests
    {
        private DbpayphoneTestContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DbpayphoneTestContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // base in-memory única para cada test
                .Options;

            return new DbpayphoneTestContext(options);
        }

        [Fact]
        public async Task CreateWalletAsync_ShouldAddWallet()
        {
            var context = GetInMemoryContext();
            var service = new WalletService(context);

            var dto = new WalletDto
            {
                DocumentId = "1234567890",
                Name = "Juan Perez",
                Balance = 100
            };


            var id = await service.CreateWalletAsync(dto);

            var wallet = await context.Wallets.FindAsync(id);
            Assert.NotNull(wallet);
            Assert.Equal("Juan Perez", wallet.Name);
            Assert.Equal(100, wallet.Balance);
        }

        [Fact]
        public async Task UpdateWalletAsync_ShouldUpdateWalletName()
        {
            var context = GetInMemoryContext();
            var service = new WalletService(context);

            var wallet = new Wallet
            {
                DocumentId = "1234567890",
                Name = "Antiguo Nombre",
                Balance = 200,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Wallets.AddAsync(wallet);
            await context.SaveChangesAsync();

            var updatedDto = new WalletDto
            {
                DocumentId = wallet.DocumentId,
                Name = "Nuevo Nombre",
                Balance = wallet.Balance
            };

            var result = await service.UpdateWalletAsync(wallet.Id, updatedDto);

            Assert.True(result);
            Assert.Equal("Nuevo Nombre", (await context.Wallets.FindAsync(wallet.Id))?.Name);
        }

        [Fact]
        public async Task DeleteWalletAsync_ShouldDeleteWallet()
        {
            var context = GetInMemoryContext();
            var service = new WalletService(context);

            var wallet = new Wallet
            {
                DocumentId = "1234567890",
                Name = "Para eliminar",
                Balance = 50,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await context.Wallets.AddAsync(wallet);
            await context.SaveChangesAsync();

            var result = await service.DeleteWalletAsync(wallet.Id);

            Assert.True(result);
            Assert.Null(await context.Wallets.FindAsync(wallet.Id));
        }

        [Fact]
        public async Task TransferAsync_ShouldTransferFunds_WhenValid()
        {
            var context = GetInMemoryContext();
            var service = new WalletService(context);

            var from = new Wallet { DocumentId = "A", Name = "De", Balance = 100, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
            var to = new Wallet { DocumentId = "B", Name = "Para", Balance = 0, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };

            await context.Wallets.AddRangeAsync(from, to);
            await context.SaveChangesAsync();

            var dto = new TransferDto
            {
                FromWalletId = from.Id,
                ToWalletId = to.Id,
                Amount = 50,
                ToWalletName = "Para"
            };

            var result = await service.TransferAsync(dto);

            Assert.True(result.Success);
            var updatedFrom = await context.Wallets.FindAsync(from.Id);
            var updatedTo = await context.Wallets.FindAsync(to.Id);
            Assert.Equal(50, updatedFrom.Balance);
            Assert.Equal(50, updatedTo.Balance);
        }

        [Fact]
        public async Task TransferAsync_ShouldFail_WhenInsufficientFunds()
        {
            var context = GetInMemoryContext();
            var service = new WalletService(context);

            var from = new Wallet
            {
                DocumentId = "X123456789",
                Name = "Pobre",
                Balance = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var to = new Wallet
            {
                DocumentId = "Y123456789",
                Name = "Rico",
                Balance = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Wallets.AddRangeAsync(from, to);
            await context.SaveChangesAsync();

            var dto = new TransferDto
            {
                FromWalletId = from.Id,
                ToWalletId = to.Id,
                Amount = 100,
                ToWalletName = "Rico"
            };

            var result = await service.TransferAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Saldo insuficiente.", result.Message);
        }

        [Fact]
        public async Task TransferAsync_ShouldFail_WhenToWalletNameDoesNotMatch()
        {
            var context = GetInMemoryContext();
            var service = new WalletService(context);

            var from = new Wallet
            {
                DocumentId = "FROM123456",
                Name = "Remitente",
                Balance = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var to = new Wallet
            {
                DocumentId = "TO12345678",
                Name = "DestinatarioReal",
                Balance = 50,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Wallets.AddRangeAsync(from, to);
            await context.SaveChangesAsync();

            var dto = new TransferDto
            {
                FromWalletId = from.Id,
                ToWalletId = to.Id,
                Amount = 20,
                ToWalletName = "NombreIncorrecto"
            };

            var result = await service.TransferAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("El nombre de la cuenta de destino no coincide con el ID proporcionado.", result.Message);
        }

        [Fact]
        public async Task TransferAsync_ShouldSucceed_WhenAllDataIsValid()
        {
            var context = GetInMemoryContext();
            var service = new WalletService(context);

            var from = new Wallet
            {
                DocumentId = "FROM987654",
                Name = "Juan",
                Balance = 200,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var to = new Wallet
            {
                DocumentId = "TO98765432",
                Name = "Maria",
                Balance = 50,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Wallets.AddRangeAsync(from, to);
            await context.SaveChangesAsync();

            var dto = new TransferDto
            {
                FromWalletId = from.Id,
                ToWalletId = to.Id,
                Amount = 100,
                ToWalletName = "Maria"
            };

            var result = await service.TransferAsync(dto);

            Assert.True(result.Success);
            Assert.Equal("Transferencia completada exitosamente.", result.Message);
        }



        [Fact]
        public async Task CreateWalletAsync_ShouldThrow_WhenDocumentIdIsNotExactly10Characters()
        {
            var service = new WalletService(GetInMemoryContext());

            var tooShortDto = new WalletDto
            {
                DocumentId = "12345", // < 10
                Name = "Corto",
                Balance = 100
            };

            var tooLongDto = new WalletDto
            {
                DocumentId = "12345678901", // > 10
                Name = "Largo",
                Balance = 100
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateWalletAsync(tooShortDto));
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateWalletAsync(tooLongDto));
        }

        [Fact]
        public async Task TransferAsync_ShouldFail_WhenTransferringToSameWallet()
        {
            var context = GetInMemoryContext();
            var service = new WalletService(context);

            var wallet = new Wallet
            {
                DocumentId = "1234567890",
                Name = "Autotransfer",
                Balance = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Wallets.AddAsync(wallet);
            await context.SaveChangesAsync();

            var dto = new TransferDto
            {
                FromWalletId = wallet.Id,
                ToWalletId = wallet.Id,
                Amount = 10,
                ToWalletName = "Autotransfer"
            };

            var result = await service.TransferAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("No se puede transferir a la misma billetera.", result.Message);
        }

        [Fact]
        public async Task TransferAsync_ShouldFail_WhenToWalletNameIsMissing()
        {
            var context = GetInMemoryContext();
            var service = new WalletService(context);

            var from = new Wallet
            {
                DocumentId = "FROM999999",
                Name = "Origen",
                Balance = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var to = new Wallet
            {
                DocumentId = "TO99999999",
                Name = "Destino",
                Balance = 100,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.Wallets.AddRangeAsync(from, to);
            await context.SaveChangesAsync();

            var dto = new TransferDto
            {
                FromWalletId = from.Id,
                ToWalletId = to.Id,
                Amount = 10,
                ToWalletName = "" 
            };

            var result = await service.TransferAsync(dto);

            Assert.False(result.Success);
            Assert.Equal("Debe proporcionar el nombre de la cuenta de destino.", result.Message);
        }

    }

}

