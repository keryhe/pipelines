using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Keryhe.Pipelines.Steps
{
    public interface IStep<T>
    {
        void Start(Action<T, int> callback, int stepIndex, CancellationToken cancellationToken);
        void Execute(T data);

        event EventHandler<StepEventArgs<T>> StepStarted;
        event EventHandler<StepEventArgs<T>> StepCompleted;
    }

    public class StepEventArgs<T> : EventArgs
    {
        public StepEventArgs(string stepName, T data)
        {
            StepName = stepName;
            Data = data;
        }

        public string StepName { get; set; }
        public T Data { get; set; }
    }

    public class StepExceptionEventArgs<T> : StepEventArgs<T>
    {
        public StepExceptionEventArgs(string stepName, T data, Exception ex) : base(stepName, data)
        {
            Ex = ex;
        }

        public Exception Ex { get; set; }
    }
}
