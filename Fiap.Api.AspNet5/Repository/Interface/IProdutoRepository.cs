using Fiap.Api.AspNet5.Models;

namespace Fiap.Api.AspNet5.Repository.Interface
{
    public interface IProdutoRepository
    {

        public int Count();
        public IList<ProdutoModel> FindAll(int pagina, int tamanho);
        public IList<ProdutoModel> FindAll();
        public ProdutoModel FindById(int id);
        public int Insert(ProdutoModel produtoModel);
        public void Delete(int id);
        public void Update(ProdutoModel produtoModel);
    }
}
