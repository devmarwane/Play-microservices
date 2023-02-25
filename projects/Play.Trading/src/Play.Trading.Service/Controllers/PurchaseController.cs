using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Play.Trading.Service.Dtos;
using Play.Trading.Service.StateMachines;
using System.Security.Claims;

namespace Play.Trading.Service.Controllers
{
    [ApiController]
    [Route("purchase")]
    [Authorize]
    public class PurchaseController : ControllerBase
    {

        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IRequestClient<GetPurchaseState> _purchaseClient;

        public PurchaseController(IPublishEndpoint publishEndpoint, IRequestClient<GetPurchaseState> purchaseClient)
        {
            _publishEndpoint = publishEndpoint;
            _purchaseClient = purchaseClient;
        }

        [HttpGet("status/{correlationId}")]
        public async Task<ActionResult<PurchaseDto>> GetStatusAsync(Guid correlationId)
        {
            var response = await _purchaseClient.GetResponse<PurchaseState>(
                new GetPurchaseState(correlationId));

            var purchaseState = response.Message;

            var purchase = new PurchaseDto
            (
                UserId : purchaseState.UserId,
                ItemId : purchaseState.ItemId,
                Quantity : purchaseState.Quantity,
                State: purchaseState.CurrentState,
                LastUpdated : purchaseState.LastUpdated,
                Received : purchaseState.Received,
                PuchaseTotal : purchaseState.PuschaseTotal,
                Reason : purchaseState.ErrorMessage
            );

            return Ok(purchase);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(SubmitPurchaseDto purchase)
        {
            var userId = User.FindFirstValue("sub");
            var correlationId = Guid.NewGuid();

            var message = new PurchaseRequested
                (
                Guid.Parse(userId),
                purchase.ItemId.Value,
                purchase.Quantity,
                correlationId
                );

            await _publishEndpoint.Publish<PurchaseRequested>(message);

            return AcceptedAtAction(nameof(GetStatusAsync), new { correlationId }, new { correlationId });

        }


    }
}