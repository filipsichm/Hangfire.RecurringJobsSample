namespace BusinessLogic
{
    public interface ITaskHandlerDispatcher
    {
        ITaskHandler Dispatch(string handlerTypeName);
    }
}
