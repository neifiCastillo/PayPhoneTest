using Microsoft.EntityFrameworkCore;
using PeyphoneTest.Models;
using PeyphoneTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeyphoneTest.UnitTests.Services
{
    public class HistoryServiceTests
    {
        private DbpayphoneTestContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DbpayphoneTestContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new DbpayphoneTestContext(options);
        }

        [Fact]
        public async Task GetHistoryByIdAsync_ShouldReturnHistoryForWallet()
        {
            var context = GetInMemoryContext();
            var walletId = 1;

            context.HistoryMovements.AddRange(new[]
            {
                new HistoryMovement { WalletId = walletId, Amount = 100, Type = "Crédito", CreatedAt = DateTime.UtcNow },
                new HistoryMovement { WalletId = walletId, Amount = 50, Type = "Débito", CreatedAt = DateTime.UtcNow },
                new HistoryMovement { WalletId = 999, Amount = 200, Type = "Crédito", CreatedAt = DateTime.UtcNow }
            });

            await context.SaveChangesAsync();

            var service = new HistoryService(context);
            var result = await service.GetHistoryByIdAsync(walletId);

            Assert.Equal(2, result.Count);
            Assert.All(result, h => Assert.Equal(walletId, walletId));
        }
    }
}
