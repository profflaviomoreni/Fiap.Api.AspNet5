using Fiap.Api.AspNet5.Data;
using Fiap.Api.AspNet5.Models;
using Fiap.Api.AspNet5.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Api.AspNet5.Repository
{
    public class UsuarioRepository : IUsuarioRepository
    {

        private readonly DataContext context;

        public UsuarioRepository(DataContext _context)
        {
            context = _context;
        }

        public IList<UsuarioModel> FindAll()
        {
            return context.Usuarios.AsNoTracking().ToList();
        }

        public UsuarioModel FindById(int id)
        {
            return context.Usuarios.FirstOrDefault(x => x.UsuarioId == id);
        }

        public UsuarioModel FindByName(string name)
        {
            return context.Usuarios.FirstOrDefault(x => x.NomeUsuario == name);
        }


        public int Insert(UsuarioModel usuarioModel)
        {
            context.Usuarios.Add(usuarioModel);
            context.SaveChanges();
            return usuarioModel.UsuarioId;
        }

        public void Update(UsuarioModel usuarioModel)
        {
            context.Usuarios.Update(usuarioModel);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            var usuario = new UsuarioModel()
            {
                UsuarioId = id
            };

            context.Usuarios.Remove(usuario);
            context.SaveChanges();
        }

    }
}
