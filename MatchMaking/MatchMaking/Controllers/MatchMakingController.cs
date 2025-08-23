using AutoMapper;
using DataLayer.Entities;
using GenericCrud;
using MatchMaking.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceLayer.Configuration;
using ServiceLayer.Models;
using ServiceLayer.Services;
using System.Security.Claims;

namespace MatchMaking.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class MatchMakingController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICrudService<Lobby, LobbyDto, int> _lobbyService;
        private readonly MatchingService _matchingService;
        private readonly TimeSpan _lobbyExpirationTime;

        public MatchMakingController(IMapper mapper, ICrudService<Lobby, LobbyDto, int> lobbyService, MatchingService matchingService, IOptions<LobbySettings> options)
        {
            _mapper = mapper;
            _lobbyService = lobbyService;
            _matchingService = matchingService;
            _lobbyExpirationTime = TimeSpan.FromSeconds(options.Value.ExpirationTimeSeconds);
        }


        [HttpGet("open")]
        public async Task<IEnumerable<OpenLobbyDto>> GetOpenLobbies(CancellationToken cancellationToken)
        {
            IEnumerable<LobbyDto> lobbies = await _lobbyService.GetAsync(
                filter: l => l.ExpirationTime < DateTime.UtcNow,
                cancellationToken);
            return _mapper.Map<IEnumerable<OpenLobbyDto>>(lobbies);
        }


        [HttpPost("open")]
        public async Task<int> CreateOpenLobby([FromBody] CreateOpenLobbyRequest request)
        {
            LobbyDto lobby = _mapper.Map<LobbyDto>(request);
            lobby.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            lobby.ExpirationTime = DateTime.UtcNow + _lobbyExpirationTime;

            int id = await _lobbyService.CreateAsync(lobby);
            return id;
        }


        [HttpPost("private")]
        public async Task<int> CreatePrivateLobby([FromBody] CreatePrivateLobbyRequest request)
        {
            LobbyDto lobby = _mapper.Map<LobbyDto>(request);
            lobby.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            lobby.ExpirationTime = DateTime.UtcNow + _lobbyExpirationTime;

            int id = await _lobbyService.CreateAsync(lobby);
            return id;
        }


        [HttpPost("open/join/{id}"), LobbyExist]
        public async Task<IActionResult> JoinOpenLobby(int id)
        {
            LobbyDto lobby = await _lobbyService.GetAsync(id);
            string userJoiningId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            _matchingService.CreateGame(lobby, userJoiningId);
            await _lobbyService.DeleteAsync(lobby.Id);
            return Ok();
        }


        [HttpPost("private/join/{id}"), LobbyExist]
        public async Task<IActionResult> JoinByInventation(int id)
        {
            LobbyDto lobby = await _lobbyService.GetAsync(id);
            string userJoiningId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            if (userJoiningId != lobby.InvitedId)
            {
                return Forbid();
            }

            _matchingService.CreateGame(lobby, userJoiningId);
            await _lobbyService.DeleteAsync(lobby.Id);
            return Ok();
        }

        [HttpPatch("{id}"), LobbyExist]
        public async Task<IActionResult> UpdateLifetime(int id)
        {
            LobbyDto lobbyDto = await _lobbyService.GetAsync(id);

            if (lobbyDto.OwnerId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            lobbyDto.ExpirationTime = DateTime.UtcNow + _lobbyExpirationTime;
            await _lobbyService.UpdateAsync(id, lobbyDto);
            return Ok();
        }


        [HttpDelete("{id}"), LobbyExist]
        public async Task<IActionResult> DeleteLobby(int id)
        {
            LobbyDto lobbyDto = await _lobbyService.GetAsync(id, CancellationToken.None);

            if (lobbyDto.OwnerId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            await _lobbyService.DeleteAsync(id);
            return Ok();
        }
    }
}
