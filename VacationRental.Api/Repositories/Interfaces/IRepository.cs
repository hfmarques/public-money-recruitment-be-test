namespace VacationRental.Api.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        T GetById(int id);
        int Add(T t);
        void Update(T t);
    }
}