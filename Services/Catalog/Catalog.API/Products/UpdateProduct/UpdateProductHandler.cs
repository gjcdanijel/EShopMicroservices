using Catalog.API.Models;
using Common.CQRS;
using FluentValidation;
using Marten;

namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductCommand(Guid Id,
    string Name,
    List<string> Category,
    string Description,
    string ImageFile,
    decimal Price) : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Id).NotNull().WithMessage("Product ID is required");

        RuleFor(command => command.Name)
            .NotNull().WithMessage("Product Name is required")
            .Length(2, 150).WithMessage("Price must be grater than 0");
        
        RuleFor(command => command.Price)
            .GreaterThan(0).WithMessage("Price must be grater than 0");
    }
}
internal class UpdateProductCommandHandler
(IDocumentSession session,  ILogger<UpdateProductCommandHandler> logger)
: ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("UpdateProductHandler.Handle called with {@Command}", command);
        
        var product = await session.LoadAsync<Product>(command.Id, cancellationToken);
        
        if(product is null)
        {
            return new UpdateProductResult(false);
        }
        
        product.Name = command.Name;
        product.Category = command.Category;
        product.Description = command.Description;
        product.ImageFile = command.ImageFile;
        product.Price = command.Price;
        
        session.Update(product);   
        await session.SaveChangesAsync(cancellationToken);
        
        return new UpdateProductResult(true);
    }
}