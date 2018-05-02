using LinFu.IoC;
using LinFu.IoC.Configuration;

namespace ERP.Infrastructure.IoC
{
    /// <summary>
    /// Контейнер IoC
    /// </summary>
    public static class IoCContainer
    {
        public static ServiceContainer Container { get; set; }
        
        static IoCContainer()
        {
            Container = new ServiceContainer();
        }
        
        /// <summary>
        /// Регистрация связи в контейнере
        /// </summary>
        /// <typeparam name="IT">Реализуемый интерфейс</typeparam>
        /// <param name="O">Объект класса, реализующего интерфейс</param>
        public static void Register<IT>(IT O)
        {
            Container.AddService(typeof(IT), O);
        }
        
        /// <summary>
        /// Регистрация связи типа «синглтон» в контейнере
        /// </summary>
        /// <typeparam name="IT">Реализуемый интерфейс</typeparam>
        /// <typeparam name="TT">Класс, реализующий интерфейс</typeparam>
        public static void RegisterSingleton<IT, TT>()
        {
            Container.AddService(typeof(IT), typeof(TT), LifecycleType.Singleton); 
        }

        /// <summary>
        /// Получение объекта по типу реализуемого интерфейса
        /// </summary>
        /// <typeparam name="IT">Реализуемый интерфейс</typeparam>
        /// <returns>Объект класса, реализующего заданный интерфейс</returns>
        public static IT Resolve<IT>()
        {
            return (IT)Container.GetService(typeof(IT));
        }
    }
}
