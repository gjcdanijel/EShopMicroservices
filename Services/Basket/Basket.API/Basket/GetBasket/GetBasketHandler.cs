using Basket.API.Models;
using Common.CQRS;

namespace Basket.API.Basket.GetBasket;

public record GetBasketQuery(string UserName) : IQuery<GetBasketResult>;

public record GetBasketResult(ShoppingCart Cart);

public class GetBasketHandler : IQueryHandler<GetBasketQuery, GetBasketResult>
{
    public async Task<GetBasketResult> Handle(GetBasketQuery query, CancellationToken cancellationToken)
    {
        // Todo: get basket from db
        //var basket=await _repository.GetBasket(request.UserName);

        return new GetBasketResult(new ShoppingCart("test"));
    }
}