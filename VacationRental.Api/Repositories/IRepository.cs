namespace VacationRental.Api.Repositories
{
    public interface IRepository<T>
    {
        T GetById(int id);
        int Add(T t);
        void Update(T t);
    }
}