namespace BankAccount.Api.Controllers;

using BankAccount.Api.Dtos;
using BankAccount.Core.Interfaces;
using BankAccount.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/accounts")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _service;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        IAccountService service,
        ILogger<AccountController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost("withdraw")]
    public async Task<ActionResult<WithdrawalResponse>> Withdraw(
        [FromBody] WithdrawalRequest request)
    {
        try
        {
            await _service.ProcessWithdrawalAsync(request.AccountId, request.Amount, request.modifiedBy);
            return Ok(new WithdrawalResponse("Success", "Withdrawal processed"));
        }
        catch (InsufficientFundsException ex)
        {
            _logger.LogWarning(ex, "Withdrawal rejected");
            return BadRequest(new WithdrawalResponse("Failed", ex.Message));
        }
        catch (AccountNotFoundException ex)
        {
            _logger.LogWarning(ex, "Account not found");
            return NotFound(new WithdrawalResponse("Failed", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Withdrawal error");
            return StatusCode(500, new WithdrawalResponse("Error", "Processing failed"));
        }
    }
}