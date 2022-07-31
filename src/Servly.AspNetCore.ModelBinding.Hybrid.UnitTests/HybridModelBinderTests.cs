using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using Shouldly;
using Xunit;

namespace Servly.AspNetCore.ModelBinding.Hybrid.UnitTests;

public class HybridModelBinderTests
{
    [Fact]
    public async Task ShouldSetModelWhenBodyResultModelIsSet()
    {
        var mockBodyBinder = new Mock<IModelBinder>();
        mockBodyBinder
            .Setup(x => x.BindModelAsync(It.IsAny<ModelBindingContext>()))
            .Callback<ModelBindingContext>(context =>
            {
                context.Result = ModelBindingResult.Success("Result");
            });

        var mockComplexBinder = new Mock<IModelBinder>();
        mockComplexBinder
            .Setup(x => x.BindModelAsync(It.IsAny<ModelBindingContext>()))
            .Callback<ModelBindingContext>(context =>
            {
                context.Model.ShouldBe("Result");
            });

        var sut = new HybridModelBinder(mockBodyBinder.Object, mockComplexBinder.Object);

        var context = new DefaultModelBindingContext();
        await sut.BindModelAsync(context);

        context.Model.ShouldBe("Result");

        mockBodyBinder
            .Verify(x => x.BindModelAsync(It.IsAny<ModelBindingContext>()), Times.Once);

        mockComplexBinder
            .Verify(x => x.BindModelAsync(It.IsAny<ModelBindingContext>()), Times.Once);
    }

    [Fact]
    public async Task ShouldNotSetModelWhenBodyResultModelIsNotSet()
    {
        var mockBodyBinder = new Mock<IModelBinder>();
        mockBodyBinder
            .Setup(x => x.BindModelAsync(It.IsAny<ModelBindingContext>()))
            .Callback<ModelBindingContext>(context =>
            {
                context.Result = ModelBindingResult.Failed();
            });

        var mockComplexBinder = new Mock<IModelBinder>();
        mockComplexBinder
            .Setup(x => x.BindModelAsync(It.IsAny<ModelBindingContext>()))
            .Callback<ModelBindingContext>(context =>
            {
                context.Model.ShouldNotBe("Result");
                context.Model.ShouldBeNull();

                context.Result = ModelBindingResult.Success("SecondResult");
            });

        var sut = new HybridModelBinder(mockBodyBinder.Object, mockComplexBinder.Object);

        var context = new DefaultModelBindingContext();
        await sut.BindModelAsync(context);

        context.Result.Model.ShouldBe("SecondResult");

        mockBodyBinder
            .Verify(x => x.BindModelAsync(It.IsAny<ModelBindingContext>()), Times.Once);

        mockComplexBinder
            .Verify(x => x.BindModelAsync(It.IsAny<ModelBindingContext>()), Times.Once);
    }
}
