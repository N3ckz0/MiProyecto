using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MiProyecto.Web.Layout
{
    public partial class NavMenu
    {
        [Inject] public required NavigationManager Navigation { get; set; }
        [Inject] public required IJSRuntime JS { get; set; }

        private bool collapseNavMenu = true;

        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }

        protected async Task Logout()
        {
            await JS.InvokeVoidAsync("localStorage.removeItem", "token");
            Navigation.NavigateTo("/", true);
        }
    }
}