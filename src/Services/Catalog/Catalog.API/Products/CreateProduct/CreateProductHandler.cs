﻿using Microsoft.Extensions.Logging;

namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand(
	string Name,
	List<string> Category,
	string Description,
	string ImageFile,
	decimal Price)
	: ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
	public CreateProductCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty().WithMessage("Name is requeired");
		RuleFor(x => x.Category).NotEmpty().WithMessage("Category is requeired");
		RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile is requeired");
		RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price is requeired");
	}
}

internal class CreateProductCommandHandler
	(IDocumentSession session)
: ICommandHandler<CreateProductCommand, CreateProductResult>
{
	public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
	{
		var product = new Product
		{
			Name = command.Name,
			Category = command.Category,
			Description = command.Description,
			ImageFile = command.ImageFile,
			Price = command.Price
		};

		session.Store(product);
		await session.SaveChangesAsync(cancellationToken);

		return new CreateProductResult(product.Id);
	}
}