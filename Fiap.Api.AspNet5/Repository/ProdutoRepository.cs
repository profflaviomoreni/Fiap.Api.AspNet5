using Fiap.Api.AspNet5.Data;
using Fiap.Api.AspNet5.Models;
using Fiap.Api.AspNet5.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Api.AspNet5.Repository
{
    public class ProdutoRepository : IProdutoRepository
    {
        private readonly DataContext context;

        public ProdutoRepository(DataContext _context)
        {
            context = _context;
        }

        public IList<ProdutoModel> FindAll()
        {
            return context.Produtos.Include(c => c.Categoria)
                                    .Include(m => m.Marca)
                                    .AsNoTracking()
                                    .ToList();
        }



        public ProdutoModel FindById(int id)
        {
            return context.Produtos
                        .Include(c => c.Categoria)
                        .Include(m => m.Marca)
                            .FirstOrDefault(x => x.ProdutoId == id);


        }

        public int Insert(ProdutoModel produtoModel)
        {
            context.Produtos.Add(produtoModel);
            context.SaveChanges();
            return produtoModel.ProdutoId;
        }

        public void Update(ProdutoModel produtoModel)
        {
            context.Produtos.Update(produtoModel);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var produto = new ProdutoModel()
            {
                ProdutoId = id
            };

            context.Produtos.Remove(produto);
            context.SaveChanges();
        }

        public int Count()
        {
            return context.Produtos.Count();
        }

        public IList<ProdutoModel> FindAll(int pagina, int tamanho)
        {
            var lista = context.Produtos
                            .Skip(tamanho * pagina)
                            .Take(tamanho).ToList();
            return lista;
        }
    }
}
