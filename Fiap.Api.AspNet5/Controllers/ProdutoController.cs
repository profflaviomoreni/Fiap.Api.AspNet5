using Fiap.Api.AspNet5.Models;
using Fiap.Api.AspNet5.Repository.Interface;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.Api.AspNet5.Controllers
{

    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [ApiVersion("3.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProdutoController : ControllerBase
    {

        private readonly IProdutoRepository produtoRepository;

        public ProdutoController(IProdutoRepository _produtoRepository)
        {
            produtoRepository = _produtoRepository;        
        }



        
        [HttpGet]
        [ApiVersion("1.0", Deprecated = true)]
        public ActionResult<IList<ProdutoModel>> Get()
        {
            var produto = produtoRepository.FindAll();

            if (produto.Count == 0)
            {
                return NoContent();
            }

            return Ok(produto);
        }




        /// <summary>
        ///     Resumo do método GET da API de Produto
        /// </summary>
        /// <param name="pagina">Recebe qual a página que eu quero consulta das produto</param>
        /// <param name="tamanho">Quantidade de itens exibindo na consulta</param>
        /// <returns>200 Sucesso, 404 Nada encontrado, 403 acesso negado</returns>
        [HttpGet]
        [ApiVersion("2.0")]
        [ApiVersion("3.0")]
        public ActionResult<dynamic> Get(
            [FromQuery] int pagina = 0, 
            [FromQuery] int tamanho = 3)
        {

            var totalGeral = produtoRepository.Count();
            var totalPaginas = Convert.ToInt16( Math.Ceiling( (double) totalGeral / tamanho) );
            var anterior = (pagina > 0 ) ? $"produto?pagina={pagina - 1}&tamanho={tamanho}"  : "";
            var proximo = (pagina < totalPaginas - 1) ? $"produto?pagina={pagina + 1}&tamanho={tamanho}" : "" ;

            if ( pagina > totalPaginas )
            {
                return NotFound();
            }

            var produtos = produtoRepository.FindAll(pagina, tamanho);

            var retorno = new
            {
                total = totalGeral,
                totalPaginas = totalPaginas,
                anterior = anterior,
                proximo = proximo,
                produtos = produtos
            };

            return Ok(retorno);
        }



        [HttpGet("{id:int}")]
        public ActionResult<ProdutoModel> GetById([FromRoute] int id)
        {
            var produto = produtoRepository.FindById(id);

            if (produto == null)
            {
                return NotFound();
            }

            return Ok(produto);
        }

        [HttpPost]
        public ActionResult<ProdutoModel> Post([FromBody] ProdutoModel produtoModel)
        {

            try
            {
                var produtoId = produtoRepository.Insert(produtoModel);
                produtoModel.ProdutoId = produtoId;

                var location = new Uri(Request.GetEncodedUrl() + produtoId);

                return Created(location, produtoModel);
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"Não foi possível inserir o produto. Detalhes: {error.Message}" });
            }
        }

        [HttpPut("{id:int}")]
        public ActionResult<ProdutoModel> Put(
            [FromRoute] int id,
            [FromBody] ProdutoModel produtoModel)
        {


            if (produtoModel.ProdutoId != id)
            {
                return NotFound();
            }

            try
            {
                produtoRepository.Update(produtoModel);

                return Ok(produtoModel);
            }
            catch (Exception error)
            {
                return BadRequest(new { message = $"Não foi possível alterar o produto. Detalhes: {error.Message}" });
            }
        }


        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoModel> Delete([FromRoute] int id)
        {

            produtoRepository.Delete(id);

            return Ok();
        }
    }
}
