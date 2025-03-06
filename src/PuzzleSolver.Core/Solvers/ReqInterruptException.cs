namespace PuzzleSolver.Core.Solvers;

public class ReqInterruptException : Exception
{
    public ReqInterruptException(string message) : base(message)
    {
    }
}