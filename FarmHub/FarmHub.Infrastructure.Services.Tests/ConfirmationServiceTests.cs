using System;
using System.Threading.Tasks;
using FarmHub.Application.Services.Repositories;
using FarmHub.Data.Models;
using FarmHub.Domain.Services;
using FarmHub.Domain.Services.Repositories;
using FluentAssertions;
using Xunit;

namespace FarmHub.Infrastructure.Services
{
    public class ConfirmationServiceTests : DbContextTestBase
    {
        /* ValidateGuid
         * Case 1: Happy path: Confirmation exists, valid, (Forget Email, Confirmation Email)
         * Case 2: Confirmation exists but expired
         * Case 3: Confirmation GUID does not exist
         */

        [Theory]
        [InlineData("65a85a33-e5c2-45b4-8cbb-57a928c7fbdc", "65a85a33-e5c2-45b4-8cbb-57a928c7fbdc", 1, ConfirmationEmailType.AccountCreation,  ConfirmationEmailType.AccountCreation, ConfirmationResult.Success)]
        [InlineData("65a85a33-e5c2-45b4-8cbb-57a928c7fbdc", "65a85a33-e5c2-45b4-8cbb-57a928c7fbdc", 1, ConfirmationEmailType.AccountCreation,  ConfirmationEmailType.ForgotPassword, ConfirmationResult.NonExistent)]
        [InlineData("65a85a33-e5c2-45b4-8cbb-57a928c7fbdc", "4fea5e9c-b1b7-4e9a-b229-165c7934aa0b", 1, ConfirmationEmailType.AccountCreation,  ConfirmationEmailType.AccountCreation, ConfirmationResult.NonExistent)]
        [InlineData("65a85a33-e5c2-45b4-8cbb-57a928c7fbdc", "65a85a33-e5c2-45b4-8cbb-57a928c7fbdc", -1, ConfirmationEmailType.AccountCreation,  ConfirmationEmailType.AccountCreation, ConfirmationResult.Expired)]
        public async Task ValidateGuid_GivenParameters_ShouldMatchExpectedOutput(
            string guidSaved, string guid, int daysFromToday,ConfirmationEmailType typeInDb, ConfirmationEmailType type, ConfirmationResult expectedValidationResult)
        {
            var confirmation = new ConfirmationEmail
            {
                Email = "andrei@pogi.ph",
                Guid = Guid.Parse(guidSaved),
                Expiration = DateTime.Now.AddDays(daysFromToday),
                Type = typeInDb,
                IsActive = true
            };
            
            await _context.ConfirmationEmails.AddAsync(confirmation);

            await _context.AuthUsers.AddAsync(new AuthUser
            {
                Email = "andrei@pogi.ph"
            });
            
            await _context.SaveChangesAsync();
            
            var service = new ConfirmationService(_context);
            var result = await service.ValidateGuid(Guid.Parse(guid), type);
            result.Should().Be(expectedValidationResult);
        }
        
        
        // TODO Andrei: If user becomes confirmed
    }
}