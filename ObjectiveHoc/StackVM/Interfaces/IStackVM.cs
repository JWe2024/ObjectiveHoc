using IntermediateCode;

namespace StackVM.Interfaces
{
    public interface IStackVM
    {
        void Load(Program program);

        void Execute();
    }
}
