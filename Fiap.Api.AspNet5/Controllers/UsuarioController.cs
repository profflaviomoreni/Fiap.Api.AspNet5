using Fiap.Api.AspNet5.Models;
using Fiap.Api.AspNet5.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Api.AspNet5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private readonly IUsuarioRepository usuarioRepository;

        public UsuarioController(IUsuarioRepository _usuarioRepository)
        {
            usuarioRepository = _usuarioRepository;
        }


        [HttpPost]
        [Route("Login")]
        public ActionResult<UsuarioModel> Login([FromBody] UsuarioModel usuarioModel)
        {
            var usuario = usuarioRepository.FindByName(usuarioModel.NomeUsuario);

            if (usuario == null)
            {
                return NotFound();
            } 
            else if ( ! string.Equals(usuario.Senha, usuarioModel.Senha )  ) 
            { 
                return NotFound();
            } else
            {
                usuario.Senha = "";
                return Ok(usuario);
            }
             
        }


    }
}
