namespace HotelManagementSystem.Interfaces.User
{
    public interface IWaiterService
    {

    }
    public interface IWaiterDLL
    {
        Task<int?> AssignWaiterAsync();
    }
}
