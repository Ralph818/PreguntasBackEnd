using BackEnd.Domain.IRepositories;
using BackEnd.Domain.IServices;
using BackEnd.Domain.Models;

namespace BackEnd.Service
{
    public class LoginService : ILoginService
    {
        private readonly ILoginRepository _loginsRepository;
        public LoginService(ILoginRepository loginRepository)
        {

            _loginsRepository = loginRepository;

        }

        public async Task<Usuario> ValidateUser(Usuario usuario)
        {
            return await _loginsRepository.ValidateUser(usuario);
        }
    }
}
