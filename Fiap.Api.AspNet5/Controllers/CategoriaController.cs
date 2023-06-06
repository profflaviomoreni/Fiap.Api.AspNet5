using Fiap.Api.AspNet5.Models;
using Fiap.Api.AspNet5.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Api.AspNet5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaRepository categoriaRepository;

        public CategoriaController(ICategoriaRepository _categoriaRepository)
        {
            categoriaRepository = _categoriaRepository;        
        }


        [HttpGet]
        public IList<CategoriaModel> Get()
        {
            var lista = categoriaRepository.FindAll();
            return lista;
        }


        [HttpGet("{id}")]
        public CategoriaModel Get([FromRoute] int id)
        {
            var categoria = categoriaRepository.FindById(id);
            return categoria;
        }


        [HttpPost]
        public int Post( [FromBody] CategoriaModel categoriaModel) { 
            categoriaRepository.Insert(categoriaModel);
            return categoriaModel.CategoriaId;
        }


    }
}
