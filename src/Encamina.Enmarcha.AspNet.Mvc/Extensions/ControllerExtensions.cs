using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Encamina.Enmarcha.AspNet.Mvc.Extensions;

/// <summary>
/// Extension methods for controllers.
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Renders a main (not partial) view asynchronously.
    /// </summary>
    /// <typeparam name="TModel">The type of the model associated with the view.</typeparam>
    /// <param name="controller">The controller.</param>
    /// <param name="viewName">The view name.</param>
    /// <param name="model">The model for the view.</param>
    /// <returns>The rendered view, or a message if the view could not be found by the given <paramref name="viewName"/>.</returns>
    public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model)
    {
        return await RenderViewAsync(controller, viewName, model, false);
    }

    /// <summary>
    /// Renders a view asynchronously.
    /// </summary>
    /// <typeparam name="TModel">The type of the model associated with the view.</typeparam>
    /// <param name="controller">The controller.</param>
    /// <param name="viewName">The view name.</param>
    /// <param name="model">The model for the view.</param>
    /// <param name="partial">Determines if the page being found is a partial view or not.</param>
    /// <returns>The rendered view, or a message if the view could not be found by the given <paramref name="viewName"/>.</returns>
    public static async Task<string> RenderViewAsync<TModel>(this Controller controller, string viewName, TModel model, bool partial)
    {
        if (string.IsNullOrWhiteSpace(viewName))
        {
            viewName = controller.ControllerContext.ActionDescriptor.ActionName;
        }

        var viewResult = controller.HttpContext.RequestServices.GetRequiredService<ICompositeViewEngine>().FindView(controller.ControllerContext, viewName, !partial);

        if (!viewResult.Success)
        {
            return $@"A view with the name {viewName} could not be found";
        }

        controller.ViewData.Model = model;

        using var writer = new StringWriter();

        var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, writer, new HtmlHelperOptions());

        await viewResult.View.RenderAsync(viewContext);

        return writer.GetStringBuilder().ToString();
    }
}
