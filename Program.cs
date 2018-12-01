using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace IoC_Training
{
    class User
    {
        public Guid Id { get; protected set; }
        public string Email { get; protected set; }
        public string Name { get; protected set; }
        public string Password { get; protected set; }

        public User(string email, string name, string password)
        {
            Id = Guid.NewGuid();
            Email = email;
            Name = name;
            Password = password;
        }
    }

    interface IUserRepository
    {
        void Add(User user);
        User Get(string email);
        IEnumerable<User> GetAll();
    }

    class InMemoryUserRepository : IUserRepository
    {
        private List<User> usersList;

        public InMemoryUserRepository()
        {
            usersList = UserRepositoryFactory.CreateUserDB();
        }
        public void Add(User user)
            => usersList.Add(user);
        
        public User Get(string email)
        {
            var user = usersList.FirstOrDefault(x => x.Email == email);
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            var users = usersList.ToList();
            return users;
        }
    }

    static class UserRepositoryFactory
    {
        public static List<User> CreateUserDB()
        {
            return new List<User>();
        }
    }

    interface IUserService
    {
        void Add(string email, string name, string password);
        User Get(string email);
        IEnumerable<User> GetAll();
    }

    class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public void Add(string email, string name, string password)
            => _userRepository.Add(new User(email, name, password));
        
        public User Get(string email)
        {
            var user = _userRepository.Get(email);
            return user;
        }

        public IEnumerable<User> GetAll()
        {
            var users = _userRepository.GetAll();
            return users;
        }
    }

    static class IoC
    {
        public static void Run()
        {
            var container = CreateContainerIoC();
            var UserService = container.Resolve<IUserService>();
            UserService.Add("user1@email.com", "user1", "secret");
            UserService.Add("user2@email.com", "user2", "secret");

            var user = UserService.Get("user2@email.com");
            Console.WriteLine($"User ID: {user.Id}, Name: {user.Name}, Email: {user.Email}, Password: {user.Password}");
            Console.WriteLine("---");
            var users = UserService.GetAll();

            foreach(var usr in users)
            {
                Console.WriteLine($"User ID: {usr.Id}, Name: {usr.Name}, Email: {usr.Email}, Password: {usr.Password}");
            }

            Console.WriteLine("Container is fine!");
        }

        public static IContainer CreateContainerIoC()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<UserService>().As<IUserService>();
            builder.RegisterType<InMemoryUserRepository>().As<IUserRepository>();
            
            return builder.Build();
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            IoC.Run();
        }
    }
}
