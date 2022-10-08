using System.Collections.Generic;

namespace VacationRental.Api.Repositories.Abstractions
{
    public interface IRepository<T>
    {
        T GetById(int id);
        int Add(T t);
        void Update(T t);
    }
}