using BackEnd.Domain.IServices;
using BackEnd.Domain.Models;
using BackEnd.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RespuestaCuestionarioController : ControllerBase
    {
        private readonly IRespuestaCuestionarioService _respuestaCuestionarioService;
        private readonly ICuestionarioService _cuestionarioService;
        public RespuestaCuestionarioController(IRespuestaCuestionarioService respuestaCuestionarioService, ICuestionarioService cuestionarioService)
        {
            _respuestaCuestionarioService = respuestaCuestionarioService;
            _cuestionarioService = cuestionarioService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RespuestaCuestionario respuestaCuestionario)
        {
            try
            {
                await _respuestaCuestionarioService.SaveRespuestaCuestionario(respuestaCuestionario);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }

        }

        [HttpGet("{idCuestionario}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get(int idCuestionario)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = JwtConfigurator.GetTokenIdUsuario(identity);
                var listRespuestaCuestionario = await _respuestaCuestionarioService.ListRespuestaCuestionario(idCuestionario, idUsuario);
                if (listRespuestaCuestionario == null)
                    return BadRequest(new { message = "No hay respuestas para este cuestionario" });
                else
                    return Ok(listRespuestaCuestionario);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{idRespuestaCuestionario}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(int idRespuestaCuestionario)
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                int idUsuario = JwtConfigurator.GetTokenIdUsuario(identity);
                var respuestaCuestionario = await _respuestaCuestionarioService.BuscarRespuestaCuestionario(idRespuestaCuestionario, idUsuario);
                if (respuestaCuestionario == null)
                {
                    return BadRequest(new { message = "No se pudo elimnar el registro" });
                }
                else
                {
                    await _respuestaCuestionarioService.EliminarRespuestaCuestionario(respuestaCuestionario);
                    return Ok(new {message = "El registro fue eliminado con éxito"});
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);        
            }
        }

        [Route("GetCuestionarioByIdRespuesta/{idRespuestaCuestionario}")]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCuestionarioByIdRespuesta(int idRespuestaCuestionario)
        {
            try
            {
                // Obtener el idCuestionario, dado un idRespuesta
                int idCuestionario = await _respuestaCuestionarioService.GetIdCuestionarioByIdRespuesta(idRespuestaCuestionario);

                // Buscamos el cuestionario (ya lo tenemos)
                var cuestionario = await _cuestionarioService.GetCuestionario(idCuestionario);


                // Buscamos las respuestas seleccionadas dado el idRespuesta
                var listRespuestas = await _respuestaCuestionarioService.GetListRespuestas(idRespuestaCuestionario);
                return Ok(new {cuestionario = cuestionario, respuestas = listRespuestas});

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
