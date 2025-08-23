using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Models;
using DataLayer.Entities;
using GenericCrud;

namespace MatchMaking.Filters
{
    public class LobbyExistAttribute : TypeFilterAttribute
    {
        public LobbyExistAttribute() : base(typeof(LobbyExistFilter))
        { }
    }

    public class LobbyExistFilter : IAsyncActionFilter
    {
        private readonly ICrudService<Lobby, LobbyDto, int> _lobbyService;

        public LobbyExistFilter(ICrudService<Lobby, LobbyDto, int> lobbyService)
        {
            _lobbyService = lobbyService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            int lobbyId = (int)context.ActionArguments["id"];
            LobbyDto? lobby = await _lobbyService.GetAsync(lobbyId);
            if (lobby == null)
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                await next();
            }
        }
    }
}
