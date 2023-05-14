using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AdaDanaService.Data;
using AdaDanaService.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdaDanaService.Controller
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ManagerController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public ManagerController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IEnumerable<ReadUserDto>>> AllUsers()
        {
            var userItem = await _userService.GetAllUser();
            var productReadDtoList = _mapper.Map<IEnumerable<ReadUserDto>>(userItem);
            return Ok(productReadDtoList);
        }

        [HttpGet]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IEnumerable<UserBelanceDto>>> AllUsersBalance()
        {
            var userItems = await _userService.GetAllUserBelance();
            var userBalanceDtos = _mapper.Map<IEnumerable<UserBelanceDto>>(userItems);

            return Ok(userBalanceDtos);
        }

    }
}